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

namespace Services.Services.AccountService;

public class AccountService : IAccountService
{
    private readonly IAccountRepo _accountRepository;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public AccountService(IAccountRepo accountRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, IEmailService emailService)
    {
        _accountRepository = accountRepository;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _emailService = emailService;
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

        // Calculate Element and LifePalace
        int birthYear = registerRequest.Dob.Year;
        var (canChi, nguHanh) = AmDuongNienHelper.GetNguHanh(birthYear);
        string element = AmDuongNienHelper.GetNguHanhName(nguHanh);
        string lifePalace = LifePalaceHelper.CalculateLifePalace(birthYear, registerRequest.Gender.Value);

        var customer = new Customer
        {
            CustomerId = GenerateShortGuid(),
            LifePalace = lifePalace,
            Element = element
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
            Customers = new List<Customer> { customer }
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

            var existingAccount = await _accountRepository.GetAccountByEmail(request.Email);
            if (existingAccount == null)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.Message = ResponseMessageIdentity.ACCOUNT_NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                return res;
            }

            if (existingAccount.AccountId != accountId)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                res.Message = ResponseMessageIdentity.USER_NOT_ALLOWED;
                res.StatusCode = StatusCodes.Status403Forbidden;
                return res;
            }

            _mapper.Map(request, existingAccount);

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
}
