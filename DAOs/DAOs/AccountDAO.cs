using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;

namespace DAOs.DAOs
{
    public class AccountDAO
    {
        private readonly IConfiguration _configuration;
        private readonly KoiFishPondContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountDAO(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _context = new KoiFishPondContext();
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> GetAccount(string email, string password)
        {
            var (accessToken, refreshToken) = await Login(email, password);
            return accessToken;
        }

        public async Task<(string accessToken, string refreshToken)> Login(string email, string password)
        {
            try
            {
                var user = await _context.Accounts.Where(x => x.Email.Equals(email)).FirstOrDefaultAsync();

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
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
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

        public async Task<string> Register(Request.RegisterRequest registerRequest)
        {
            try
            {
                var existingUser = await _context.Accounts.Where(x => x.Email.Equals(registerRequest.Email)).FirstOrDefaultAsync();
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

                _context.Accounts.Add(newUser);
                await _context.SaveChangesAsync();

                string accessToken = CreateToken(newUser);
                string refreshToken = GenerateRefreshToken();

                if (_httpContextAccessor.HttpContext?.Session != null)
                {
                    _httpContextAccessor.HttpContext.Session.SetString("RefreshToken", refreshToken);
                    _httpContextAccessor.HttpContext.Session.SetString("Email", registerRequest.Email);
                }

                return accessToken;
            }
            catch (Exception ex)
            {
                throw new Exception($"Registration failed: {ex.Message}");
            }
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

            var user = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            string newAccessToken = CreateToken(user);
            return newAccessToken;
        }

        public void Logout()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Session.Clear();
            }
        }

        private bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedPasswordHash);
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
    }
}

