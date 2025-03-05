using BusinessObjects.Models;
using DAOs.DAOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.Interfaces;
using Services.ApiModels;
using Services.ApiModels.Account;
using Services.Interfaces;
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
using static System.Collections.Specialized.BitVector32;
using Repositories.Helpers;
using BusinessObjects.Enums;

namespace Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepo accountRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
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
                    Role = "Member"
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
                null => throw new Exception("Account not found"),
                var user => VerifyPassword(password, user.Password) ? CreateToken(user) : throw new Exception("Invalid password")
            };
        }

        public async Task<(string accessToken, string refreshToken)> Login(string email, string password)
        {
            var user = await _accountRepository.GetAccountByEmail(email);
            if (user == null)
                throw new KeyNotFoundException("USER IS NOT FOUND");

            if (!VerifyPassword(password, user.Password))
                throw new UnauthorizedAccessException("INVALID PASSWORD");

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
                throw new Exception("Email is already in use.");

            if (!registerRequest.Gender.HasValue)
                throw new ArgumentException("Gender is required.");

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
                Role = "Member",
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
                throw new UnauthorizedAccessException("Session is not available.");

            var session = _httpContextAccessor.HttpContext.Session;
            string? refreshToken = session.GetString("RefreshToken");
            string? email = session.GetString("Email");

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(email))
                throw new UnauthorizedAccessException("Invalid session. Please log in again.");

            var user = await _accountRepository.GetAccountByEmail(email);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

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
                    res.Message = "Token xác thực không được cung cấp";
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var token = authHeader.Substring("Bearer ".Length);
                if (string.IsNullOrEmpty(token))
                {
                    res.IsSuccess = false;
                    res.Message = "Token không hợp lệ";
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var accountId = await _accountRepository.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.Message = "Token không hợp lệ hoặc đã hết hạn";
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var existingAccount = await _accountRepository.GetAccountByEmail(request.Email);
                if (existingAccount == null)
                {
                    res.IsSuccess = false;
                    res.Message = "Không tìm thấy tài khoản";
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                if (existingAccount.AccountId != accountId)
                {
                    res.IsSuccess = false;
                    res.Message = "Không có quyền chỉnh sửa tài khoản này";
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    return res;
                }

                _mapper.Map(request, existingAccount);

                await _accountRepository.UpdateAccount(existingAccount);

                res.IsSuccess = true;
                res.Message = "Cập nhật thông tin thành công";
                res.StatusCode = StatusCodes.Status200OK;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Lỗi khi cập nhật thông tin: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> ChangePassword(ChangePasswordRequest request)
        {
            var res = new ResultModel();
            try
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Token xác thực không được cung cấp",
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }

                var token = authHeader.Substring("Bearer ".Length);
                if (string.IsNullOrEmpty(token))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Token không hợp lệ",
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }

                var accountId = await _accountRepository.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Token không hợp lệ hoặc đã hết hạn",
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }

                var existingAccount = await _accountRepository.GetAccountById(accountId);
                if (existingAccount == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy tài khoản",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if (string.IsNullOrEmpty(request.OldPassword) || string.IsNullOrEmpty(request.NewPassword))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu không được để trống",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (request.NewPassword.Length < 6)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu mới phải có ít nhất 6 ký tự",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (request.NewPassword != request.ConfirmPassword)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu xác nhận không khớp",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, existingAccount.Password))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu cũ không chính xác",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (BCrypt.Net.BCrypt.Verify(request.NewPassword, existingAccount.Password))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu mới không được trùng với mật khẩu cũ",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                existingAccount.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                await _accountRepository.UpdateAccount(existingAccount);

                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Đổi mật khẩu thành công",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi đổi mật khẩu: {ex.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
