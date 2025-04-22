using BusinessObjects.Models;
using DAOs.DAOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.ApiModels;
using Services.ApiModels.Account;
using Services.Mapper;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using RegisterRequest = Services.ApiModels.Account.RegisterRequest;
using Repositories.Helpers;
using BusinessObjects.Enums;
using Services.Services.EmailService;
using Repositories.Repositories.AccountRepository;
using BusinessObjects.Exceptions;
using BusinessObjects.Constants;
using CloudinaryDotNet.Actions;
using Services.ServicesHelpers.UploadService;
using Repositories.Repositories.MasterRepository;
using Services.ApiModels.Master;
using Repositories.Repositories.CustomerRepository;
using System.Xml.Linq;
using Services.ApiModels.Customer;

namespace Services.Services.AccountService;

public class AccountService : IAccountService
{
    private readonly IAccountRepo _accountRepository;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly IUploadService _uploadService;
    private readonly IMasterRepo _masterRepo;
    private readonly ICustomerRepo _customerRepo;
    public AccountService(IAccountRepo accountRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, IEmailService emailService, IUploadService uploadService, IMasterRepo masterRepo, ICustomerRepo customerRepo)
    {
        _accountRepository = accountRepository;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _emailService = emailService;
        _uploadService = uploadService;
        _masterRepo = masterRepo;
        _customerRepo = customerRepo;
    }

    private bool VerifyPassword(string inputPassword, string storedPasswordHash)
    {
        return BCrypt.Net.BCrypt.Verify(inputPassword, storedPasswordHash);
    }


    private string CreateToken(Account user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    public static string GenerateShortGuid()
    {
        Guid guid = Guid.NewGuid();
        string base64 = Convert.ToBase64String(guid.ToByteArray());
        return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
    }

    public async Task<string> RegisterGoogleUser(string name, string email)
    {
        var existingUser = await _accountRepository.GetAccountByEmail(email);
        if (existingUser == null)
        {
            var newUser = new Account
            {
                AccountId = GenerateShortGuid(),
                UserName = name,
                Email = email,
                Password = string.Empty,
                Role = RoleEnums.Customer.ToString(),
            };

            await _accountRepository.AddAccount(newUser);
            existingUser = newUser;
        }
        return CreateToken(existingUser);
    }

    public async Task<string> GetAccount(string email, string password)
    {
        return _accountRepository.GetAccountByEmail(email).Result switch
        {
            null => throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageIdentity.INCORRECT_EMAIL, StatusCodes.Status404NotFound),
            var user => VerifyPassword(password, user.Password) ? CreateToken(user) : throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageIdentity.PASSWORD_INVALID, StatusCodes.Status400BadRequest)
        };
    }

    public async Task<(string accessToken, string refreshToken)> Login(string email, string password)
    {
        var user = await _accountRepository.GetAccountByEmail(email);
        if (user == null)
            throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstantsUser.USER_NOT_FOUND, StatusCodes.Status404NotFound);

        if (!VerifyPassword(password, user.Password))
            throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageIdentity.PASSWORD_INVALID, StatusCodes.Status400BadRequest);
        string accessToken = CreateToken(user);
        string refreshToken = GenerateRefreshToken();

        if (_httpContextAccessor.HttpContext?.Session != null)
        {
            _httpContextAccessor.HttpContext.Session.SetString("RefreshToken", refreshToken);
            _httpContextAccessor.HttpContext.Session.SetString("Email", email);
        }

        return (accessToken, refreshToken);
    }


    public async Task<string> Register(RegisterRequest registerRequest)
    {
        var existingUser = await _accountRepository.GetAccountByEmail(registerRequest.Email);
        if (existingUser != null)
            throw new AppException(ResponseCodeConstants.EXISTED, ResponseMessageIdentity.EXISTED_EMAIL, StatusCodes.Status400BadRequest);

        if (!registerRequest.Gender.HasValue)
            throw new AppException(ResponseCodeConstants.REQUIRED_INPUT, ResponseMessageIdentity.GENDER_REQUIRED, StatusCodes.Status400BadRequest);

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);


        int currentYear = DateTime.Now.Year;
        if (registerRequest.Dob.Year < 1925 || registerRequest.Dob.Year > currentYear)
            throw new AppException(
                ResponseCodeConstants.BAD_REQUEST,
                ResponseMessageIdentity.INVALID_DOB_YEAR,
                StatusCodes.Status400BadRequest);

        // Calculate Element and LifePalace
        int birthYear = registerRequest.Dob.Year;
        var (canChi, nguHanh) = AmDuongNienHelper.GetNguHanh(birthYear);
        string element = AmDuongNienHelper.GetNguHanhName(nguHanh);
        string lifePalace = LifePalaceHelper.CalculateLifePalace(birthYear, registerRequest.Gender.Value);

        var customer = new Customer
        {
            CustomerId = GenerateShortGuid(),
            LifePalace = lifePalace,
            Element = element,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ImageUrl = await _uploadService.UploadImageAsync(registerRequest.ImageUrl),
        };

        var newUser = new Account
        {
            AccountId = GenerateShortGuid(),
            UserName = registerRequest.FullName,
            FullName = registerRequest.FullName,
            Email = registerRequest.Email,
            Password = hashedPassword,
            PhoneNumber = registerRequest.PhoneNumber,
            Gender = registerRequest.Gender,
            Dob = DateOnly.FromDateTime(registerRequest.Dob), // Convert DateTime to DateOnly
            Role = RoleEnums.Customer.ToString(),
            Customers = new List<Customer> { customer },
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
        };

        // Set up the relationship
        customer.AccountId = newUser.AccountId;

        await _accountRepository.AddAccount(newUser);

        string accessToken = CreateToken(newUser);
        string refreshToken = GenerateRefreshToken();

        if (_httpContextAccessor.HttpContext?.Session != null)
        {
            _httpContextAccessor.HttpContext.Session.SetString("RefreshToken", refreshToken);
            _httpContextAccessor.HttpContext.Session.SetString("Email", registerRequest.Email);
        }

        return accessToken;
    }

    public async Task<string> RefreshAccessToken()
    {
        if (_httpContextAccessor.HttpContext == null)
            throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageIdentity.SESSION_NOT_FOUND, StatusCodes.Status404NotFound);

        var session = _httpContextAccessor.HttpContext.Session;
        string? refreshToken = session.GetString("RefreshToken");
        string? email = session.GetString("Email");

        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(email))
            throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageIdentity.SESSION_INVALID, StatusCodes.Status400BadRequest);

        var user = await _accountRepository.GetAccountByEmail(email);
        if (user == null)
            throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstantsUser.USER_NOT_FOUND, StatusCodes.Status404NotFound);

        return CreateToken(user);
    }

    public async void Logout()
    {
        _httpContextAccessor.HttpContext?.Session.Clear();
    }

    public async Task<ResultModel> EditProfile(EditProfileRequest request)
    {
        var res = new ResultModel();
        try
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                res.Message = ResponseMessageIdentity.TOKEN_NOT_SEND;
                res.StatusCode = StatusCodes.Status401Unauthorized;
                return res;
            }

            var token = authHeader.Substring("Bearer ".Length);
            if (string.IsNullOrEmpty(token))
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                res.Message = ResponseMessageIdentity.TOKEN_INVALID;
                res.StatusCode = StatusCodes.Status401Unauthorized;
                return res;
            }

            var accountId = await _accountRepository.GetAccountIdFromToken(token);
            if (string.IsNullOrEmpty(accountId))
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                res.StatusCode = StatusCodes.Status401Unauthorized;
                return res;
            }

            var existingAccount = await _accountRepository.GetAccountById(accountId);
            if (existingAccount == null)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.Message = ResponseMessageIdentity.ACCOUNT_NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                return res;
            }

            var duplicateAccount = await _accountRepository.GetAccountByUniqueFields(
                request.UserName,
                request.Email,
                request.PhoneNumber,
                request.BankId ?? 0,
                request.AccountNo ?? string.Empty,
                accountId);

            if (duplicateAccount != null)
            {
                if (duplicateAccount.UserName == request.UserName)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.EXISTED;
                    res.Message = ResponseMessageIdentity.EXISTED_USER_NAME;
                    res.StatusCode = StatusCodes.Status409Conflict;
                    return res;
                }
                if (duplicateAccount.Email == request.Email)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.EXISTED;
                    res.Message = ResponseMessageIdentity.EXISTED_EMAIL;
                    res.StatusCode = StatusCodes.Status409Conflict;
                    return res;
                }
                if (duplicateAccount.PhoneNumber == request.PhoneNumber)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.EXISTED;
                    res.Message = ResponseMessageIdentity.EXISTED_PHONE;
                    res.StatusCode = StatusCodes.Status409Conflict;
                    return res;
                }
                if (duplicateAccount.BankId == request.BankId && duplicateAccount.AccountNo == request.AccountNo)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.EXISTED;
                    res.Message = ResponseMessageIdentity.EXISTED_ACCOUNT_NO;
                    res.StatusCode = StatusCodes.Status409Conflict;
                    return res;
                }
            }

            if (existingAccount.AccountId != accountId)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                res.Message = ResponseMessageIdentity.USER_NOT_ALLOWED;
                res.StatusCode = StatusCodes.Status403Forbidden;
                return res;
            }

            if (!string.IsNullOrWhiteSpace(request.UserName))
                existingAccount.UserName = request.UserName;

            if (!string.IsNullOrWhiteSpace(request.Email))
                existingAccount.Email = request.Email;

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                existingAccount.PhoneNumber = request.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(request.FullName))
                existingAccount.FullName = request.FullName;

            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null && httpContext.Request.Form.ContainsKey("dob"))
                {
                    var dobString = httpContext.Request.Form["dob"].ToString();
                    if (!string.IsNullOrEmpty(dobString))
                    {
                        if (DateTime.TryParse(dobString, out var dateTime))
                        {
                            int year = dateTime.Year;
                            int currentYear = DateTime.Now.Year;

                            if (year < 1925 || year > currentYear)
                                throw new AppException(
                                    ResponseCodeConstants.BAD_REQUEST,
                                    ResponseMessageIdentity.INVALID_DOB_YEAR,
                                    StatusCodes.Status400BadRequest);

                            existingAccount.Dob = DateOnly.FromDateTime(dateTime);
                        }
                    }
                }
                else if (request.Dob.HasValue)
                {
                    existingAccount.Dob = request.Dob;
                }
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
            }

            if (request.Gender.HasValue)
                existingAccount.Gender = request.Gender;

            if (request.BankId.HasValue)
                existingAccount.BankId = request.BankId;

            if (!string.IsNullOrWhiteSpace(request.AccountNo))
                existingAccount.AccountNo = request.AccountNo;

            if (!string.IsNullOrWhiteSpace(request.AccountName))
                existingAccount.AccountName = request.AccountName;

            existingAccount.UpdateDate = DateTime.Now;

            var customer = await _customerRepo.GetCustomerByAccountId(existingAccount.AccountId);

            // Calculate Element and LifePalace
            if (request.Dob.HasValue && request.Gender.HasValue)
            {
                DateTime dobAsDateTime = request.Dob!.Value.ToDateTime(TimeOnly.MinValue);
                int birthYear = dobAsDateTime.Year;
                var (canChi, nguHanh) = AmDuongNienHelper.GetNguHanh(birthYear);
                string element = AmDuongNienHelper.GetNguHanhName(nguHanh);
                string lifePalace = LifePalaceHelper.CalculateLifePalace(birthYear, request.Gender.Value);

                if (customer != null)
                {
                    customer.LifePalace = lifePalace;
                    customer.Element = element;

                    if (request.ImageUrl != null && request.ImageUrl.Length > 0)
                    {
                        var imageUrl = await _uploadService.UploadImageAsync(request.ImageUrl);
                        customer.ImageUrl = imageUrl;
                    }
                    customer.UpdateDate = DateTime.Now;
                    await _customerRepo.UpdateCustomer(customer);
                }
            }

            await _accountRepository.UpdateAccount(existingAccount);

            res.IsSuccess = true;
            res.ResponseCode = ResponseCodeConstants.SUCCESS;
            res.Message = ResponseMessageIdentitySuccess.UPDATE_USER_SUCCESS;
            res.StatusCode = StatusCodes.Status200OK;
            return res;
        }
        catch (Exception ex)
        {
            res.IsSuccess = false;
            res.ResponseCode = ResponseCodeConstants.FAILED;
            res.Message = $"Lỗi khi cập nhật thông tin: {ex.Message}";
            res.StatusCode = StatusCodes.Status500InternalServerError;
            return res;
        }
    }


    public async Task<ResultModel> ChangePassword(ChangePasswordRequest request)
    {
        try
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader)
                || string.IsNullOrEmpty(authHeader)
                || !authHeader.ToString().StartsWith("Bearer "))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                    Message = ResponseMessageIdentity.TOKEN_NOT_SEND,
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            var token = authHeader.ToString().Substring("Bearer ".Length);
            var accountId = await _accountRepository.GetAccountIdFromToken(token);
            if (string.IsNullOrEmpty(accountId))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                    Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED,
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            var existingAccount = await _accountRepository.GetAccountById(accountId);
            if (existingAccount == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.NOT_FOUND,
                    Message = ResponseMessageIdentity.ACCOUNT_NOT_FOUND,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            if (!VerifyPassword(request.OldPassword, existingAccount.Password))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                    Message = ResponseMessageIdentity.OLD_PASSWORD_WRONG,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            if (VerifyPassword(request.NewPassword, existingAccount.Password))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                    Message = ResponseMessageIdentity.NEW_PASSWORD_CANNOT_MATCH,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            existingAccount.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _accountRepository.UpdateAccount(existingAccount);

            return new ResultModel
            {
                IsSuccess = true,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                Message = ResponseMessageIdentitySuccess.CHANGE_PASSWORD_SUCCESS,
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            return new ResultModel
            {
                IsSuccess = false,
                ResponseCode = ResponseCodeConstants.FAILED,
                Message = $"Lỗi khi đổi mật khẩu: {ex.Message}",
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    public async Task<ResultModel> ForgotPassword(string email)
    {
        var res = new ResultModel();
        try
        {
            var user = await _accountRepository.GetAccountByEmail(email);
            if (user == null)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.Message = ResponseMessageIdentity.INCORRECT_EMAIL;
                res.StatusCode = StatusCodes.Status404NotFound;
                return res;
            }
            var newPassword = GenerateShortGuid();

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            user.Password = hashedPassword;
            await _accountRepository.UpdateAccount(user);

            string subject = "Mật khẩu mới của bạn";
            string body = $"Mật khẩu mới của bạn là: <b>{newPassword}</b>. Hãy đăng nhập và đổi mật khẩu ngay.";
            await _emailService.SendEmail(email, subject, body);

            res.IsSuccess = true;
            res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
            res.Message = ResponseMessageIdentitySuccess.FORGOT_PASSWORD_SUCCESS;
            res.StatusCode = StatusCodes.Status200OK;
            return res;
        }
        catch (Exception ex)
        {
            res.IsSuccess = false;
            res.ResponseCode = ResponseCodeConstants.FAILED;
            res.Message = ex.Message;
            res.StatusCode = StatusCodes.Status500InternalServerError;
            return res;
        }
    }

    // Account Management (Admin only)
    public async Task<ResultModel> GetAllAccounts(string? role = null)
    {
        try
        {
            // Kiểm tra role Admin
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                    Message = ResponseMessageIdentity.TOKEN_NOT_SEND,
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            var token = authHeader.Substring("Bearer ".Length);
            var accountId = await _accountRepository.GetAccountIdFromToken(token);
            var admin = await _accountRepository.GetAccountById(accountId);

            if (admin == null || admin.Role != RoleEnums.Admin.ToString())
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FORBIDDEN,
                    Message = "Only Admin can access this feature",
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            // Kiểm tra role hợp lệ nếu có
            if (!string.IsNullOrEmpty(role) && !Enum.TryParse<RoleEnums>(role, out _))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                    Message = "Invalid role",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            // Lấy danh sách tài khoản
            var accounts = await _accountRepository.GetAllAccounts(role);
            var accountResponses = _mapper.Map<List<AccountResponse>>(accounts);

            return new ResultModel
            {
                IsSuccess = true,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                Message = string.IsNullOrEmpty(role) ?
                    "Get all accounts successfully" :
                    $"Get all accounts with role {role} successfully",
                Data = accountResponses,
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            return new ResultModel
            {
                IsSuccess = false,
                ResponseCode = ResponseCodeConstants.FAILED,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    public async Task<ResultModel> GetAccountsByRole(string role)
    {
        try
        {
            // Kiểm tra role Admin
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                    Message = ResponseMessageIdentity.TOKEN_NOT_SEND,
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            var token = authHeader.Substring("Bearer ".Length);
            var accountId = await _accountRepository.GetAccountIdFromToken(token);
            var admin = await _accountRepository.GetAccountById(accountId);

            if (admin == null || admin.Role != RoleEnums.Admin.ToString())
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FORBIDDEN,
                    Message = "Only Admin can access this feature",
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            // Kiểm tra role hợp lệ
            if (!Enum.TryParse<RoleEnums>(role, out _))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                    Message = "Invalid role",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            // Lấy danh sách tài khoản theo role
            var accounts = await _accountRepository.GetAccountsByRole(role);
            var accountResponses = _mapper.Map<List<AccountResponse>>(accounts);

            return new ResultModel
            {
                IsSuccess = true,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                Message = $"Get accounts with role {role} successfully",
                Data = accountResponses,
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            return new ResultModel
            {
                IsSuccess = false,
                ResponseCode = ResponseCodeConstants.FAILED,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    public async Task<ResultModel> ToggleAccountStatus(string accountId, bool isActive)
    {
        try
        {
            // Kiểm tra role Admin
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                    Message = ResponseMessageIdentity.TOKEN_NOT_SEND,
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            var token = authHeader.Substring("Bearer ".Length);
            var adminId = await _accountRepository.GetAccountIdFromToken(token);
            var admin = await _accountRepository.GetAccountById(adminId);

            if (admin == null || admin.Role != RoleEnums.Admin.ToString())
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FORBIDDEN,
                    Message = "Only Admin can access this feature",
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            // Không cho phép Admin vô hiệu hóa chính tài khoản của mình
            if (adminId == accountId)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                    Message = "Cannot toggle your own account status",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            // Cập nhật trạng thái tài khoản
            var updatedAccount = await _accountRepository.ToggleAccountStatus(accountId, isActive);
            if (updatedAccount == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.NOT_FOUND,
                    Message = "Account not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            var accountResponse = _mapper.Map<AccountResponse>(updatedAccount);

            return new ResultModel
            {
                IsSuccess = true,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                Message = $"Account status updated successfully. Account is now {(isActive ? "active" : "inactive")}",
                Data = accountResponse,
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            return new ResultModel
            {
                IsSuccess = false,
                ResponseCode = ResponseCodeConstants.FAILED,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    public async Task<ResultModel> DeleteAccount(string accountId)
    {
        try
        {
            // Kiểm tra role Admin
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                    Message = ResponseMessageIdentity.TOKEN_NOT_SEND,
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            var token = authHeader.Substring("Bearer ".Length);
            var adminId = await _accountRepository.GetAccountIdFromToken(token);
            var admin = await _accountRepository.GetAccountById(adminId);

            if (admin == null || admin.Role != RoleEnums.Admin.ToString())
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FORBIDDEN,
                    Message = "Only Admin can access this feature",
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            // Không cho phép Admin xóa chính tài khoản của mình
            if (adminId == accountId)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                    Message = "Cannot delete your own account",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            // Kiểm tra tài khoản tồn tại
            var account = await _accountRepository.GetAccountById(accountId);
            if (account == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.NOT_FOUND,
                    Message = "Account not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            // Xóa tài khoản
            await _accountRepository.DeleteAccount(accountId);

            return new ResultModel
            {
                IsSuccess = true,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                Message = "Account deleted successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            return new ResultModel
            {
                IsSuccess = false,
                ResponseCode = ResponseCodeConstants.FAILED,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    public async Task<ResultModel> UpdateAccountRole(string accountId, string newRole)
    {
        try
        {
            // Kiểm tra role Admin
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                    Message = ResponseMessageIdentity.TOKEN_NOT_SEND,
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            var token = authHeader.Substring("Bearer ".Length);
            var adminId = await _accountRepository.GetAccountIdFromToken(token);
            var admin = await _accountRepository.GetAccountById(adminId);

            if (admin == null || admin.Role != RoleEnums.Admin.ToString())
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FORBIDDEN,
                    Message = "Only Admin can access this feature",
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            // Không cho phép Admin thay đổi role của chính mình
            if (adminId == accountId)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                    Message = "Cannot change your own role",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            // Kiểm tra role hợp lệ
            if (!Enum.TryParse<RoleEnums>(newRole, out _))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                    Message = "Invalid role",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            if (newRole == RoleEnums.Master.ToString())
            {
                var account = await _accountRepository.GetAccountById(accountId);

                var customer = await _customerRepo.GetCustomerByAccountId(accountId);

                var newMaster = new Master
                {
                    MasterId = GenerateShortGuid(),
                    AccountId = accountId,
                    CreateDate = DateTime.Now,
                    ImageUrl = customer?.ImageUrl,
                    MasterName = account.FullName
                };

                // Lưu vào database
                var result = await _masterRepo.Create(newMaster);

                if (result == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = "Account not found",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if (customer == null) {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = "Customer not found",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                await _customerRepo.DeleteCustomer(customer.CustomerId);

                await _emailService.SendEmail(account.Email, "Chúc mừng bạn trở thành Master", 
                                                             "Chúc mừng bạn đã trở thành Master! Hãy vào update thêm thông tin." );

            }
            
            if(newRole == RoleEnums.Staff.ToString())
            {
                var customer = await _customerRepo.GetCustomerByAccountId(accountId);

                if (customer == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = "Customer not found",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                await _customerRepo.DeleteCustomer(customer.CustomerId);

                var account = await _accountRepository.GetAccountById(accountId);
                await _emailService.SendEmail(account.Email, "Chúc mừng bạn trở thành Staff",
                                                             "Chúc mừng bạn đã trở thành Staff!");
            }

            if (newRole == RoleEnums.Manager.ToString())
            {
                var customer = await _customerRepo.GetCustomerByAccountId(accountId);

                if (customer == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = "Customer not found",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                await _customerRepo.DeleteCustomer(customer.CustomerId);

                var account = await _accountRepository.GetAccountById(accountId);
                await _emailService.SendEmail(account.Email, "Chúc mừng bạn trở thành Manager",
                                                             "Chúc mừng bạn đã trở thành Manager!");
            }

            var accountUpdated = await _accountRepository.GetAccountById(accountId);

            accountUpdated.UpdateDate = DateTime.Now;

            accountUpdated.Role = newRole;

            var updatedAccount = await _accountRepository.UpdateAccount(accountUpdated);

            var accountResponse = _mapper.Map<AccountResponse>(updatedAccount);

            return new ResultModel
            {
                IsSuccess = true,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                Message = $"Account role updated successfully to {newRole}",
                Data = accountResponse,
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            return new ResultModel
            {
                IsSuccess = false,
                ResponseCode = ResponseCodeConstants.FAILED,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    public async Task<ResultModel> GetAllStaff()
    {
        var res = new ResultModel();
        try
        {
            var accounts = await _accountRepository.GetAllStaff();
            var accountResponses = _mapper.Map<List<AccountResponse>>(accounts);

            return new ResultModel
            {
                IsSuccess = true,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                Message = ResponseMessageConstantsUser.GET_USER_INFO_SUCCESS,
                Data = accountResponses,
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            return new ResultModel
            {
                IsSuccess = false,
                ResponseCode = ResponseCodeConstants.FAILED,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    public async Task<ResultModel> UpdateMasterProfile(MasterRequest request)
    {
        var res = new ResultModel();
        try
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                res.Message = ResponseMessageIdentity.TOKEN_NOT_SEND;
                res.StatusCode = StatusCodes.Status401Unauthorized;
                return res;
            }

            var token = authHeader.Substring("Bearer ".Length);
            if (string.IsNullOrEmpty(token))
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                res.Message = ResponseMessageIdentity.TOKEN_INVALID;
                res.StatusCode = StatusCodes.Status401Unauthorized;
                return res;
            }

            var accountId = await _accountRepository.GetAccountIdFromToken(token);
            if (string.IsNullOrEmpty(accountId))
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                res.StatusCode = StatusCodes.Status401Unauthorized;
                return res;
            }

            var existingAccount = await _accountRepository.GetAccountById(accountId);
            if (existingAccount == null)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.Message = ResponseMessageIdentity.ACCOUNT_NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                return res;
            }

            var master = await _masterRepo.GetMasterByAccountId(accountId);
            if (master == null)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.Message = ResponseMessageIdentity.MASTER_NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                return res;
            }

            _mapper.Map(request, master);

            master.ImageUrl = await _uploadService.UploadImageAsync(request.ImageUrl);
            master.UpdateDate = DateTime.Now;
            await _masterRepo.Update(master);

            res.IsSuccess = true;
            res.ResponseCode = ResponseCodeConstants.SUCCESS;
            res.Message = ResponseMessageIdentity.UPDATE_MASTER_SUCCESS;
            res.StatusCode = StatusCodes.Status200OK;
            return res;
        }
        catch (Exception ex)
        {
            res.IsSuccess = false;
            res.ResponseCode = ResponseCodeConstants.FAILED;
            res.Message = ex.Message;
            res.StatusCode = StatusCodes.Status500InternalServerError;
            return res;
        }
    }

    public async Task<ResultModel> GetAllCustomers()
    {
        var res = new ResultModel();
        try
        {
            var customers = await _customerRepo.GetAllCustomers();
            var customerResponses = _mapper.Map<List<CustomerResponse>>(customers);

            return new ResultModel
            {
                IsSuccess = true,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                Message = ResponseMessageConstantsUser.GET_USER_INFO_SUCCESS,
                Data = customerResponses,
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            return new ResultModel
            {
                IsSuccess = false,
                ResponseCode = ResponseCodeConstants.FAILED,
                Message = ex.Message,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
