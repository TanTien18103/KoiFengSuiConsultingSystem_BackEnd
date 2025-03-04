using BusinessObjects.Models;
using DAOs.DAOs;
using DAOs.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.Interfaces;
using Services.Interface;
using Services.Request;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(IAccountRepository accountRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
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
            var existingUser = await _accountRepository.GetAccountByEmailAsync(email);
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

                await _accountRepository.AddAccountAsync(newUser);
                existingUser = newUser;
            }
            return CreateToken(existingUser);
        }

        public async Task<string> GetAccount(string email, string password)
        {
           return _accountRepository.GetAccountByEmailAsync(email).Result switch
           {
               null => throw new Exception("Account not found"),
               var user => VerifyPassword(password, user.Password) ? CreateToken(user) : throw new Exception("Invalid password")
           };
        }

        public async Task<object> Login(string email, string password)
        {
            var user = await _accountRepository.GetAccountByEmailAsync(email);
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


        public async Task<string> Register(DAOs.Request.RegisterRequest registerRequest)
        {
            var existingUser = await _accountRepository.GetAccountByEmailAsync(registerRequest.Email);
            if (existingUser != null)
                throw new Exception("Email is already in use.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

            var newUser = new Account
            {
                AccountId = GenerateShortGuid(),
                UserName = registerRequest.FullName,
                Email = registerRequest.Email,
                Password = hashedPassword,
                Gender = registerRequest.Gender,
                Role = "Member"
            };

            await _accountRepository.AddAccountAsync(newUser);
            existingUser = newUser;

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

            var user = await _accountRepository.GetAccountByEmailAsync(email);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return CreateToken(user);
        }

        public async void Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
        }

        //public async Task<ResultModel> EditProfile(EditProfileRequest request)
        //{
        //    var res = new ResultModel();
        //    try
        //    {
        //        var currentUserIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid);
        //        if (currentUserIdClaim == null || !short.TryParse(currentUserIdClaim.Value, out short currentUserId))
        //        {
        //            res.IsSuccess = false;
        //            res.Code = (int)HttpStatusCode.BadRequest;
        //            res.Data = null;
        //            res.Message = "User is not authenticated or ID is invalid";
        //        }
        //        var existingAccount = await _accountRepository.GetAccount(currentUserId).FirstOrDefaultAsync();
        //        if (existingAccount == null)
        //        {
        //            return new ResultModel<UpdateProfileRequest>
        //            {
        //                IsSuccess = false,
        //                Code = (int)HttpStatusCode.NotFound,
        //                Message = "System account not found"
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.Code = (int)HttpStatusCode.InternalServerError;
        //        res.Data = null;
        //        res.Message = ex.Message;
        //    }
        //}
    }
}
