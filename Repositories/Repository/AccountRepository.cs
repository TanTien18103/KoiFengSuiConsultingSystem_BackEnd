using DAOs.DAOs;
using DAOs.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDAO _accountDAO;

        public AccountRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _accountDAO = new AccountDAO(configuration, httpContextAccessor);
        }

        public async Task<string> GetAccountIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            
            var accountId = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountId))
            {
                throw new Exception("Invalid token or account ID not found");
            }
            
            return accountId;
        }

        public async Task<string> RegisterGoogleUser(string name, string email)
        {
            return await _accountDAO.RegisterGoogleUser(name, email);
        }

        public async Task<string> GetAccount(string email, string password)
        {
            return await _accountDAO.GetAccount(email, password);
        }

        public async Task<(string accessToken, string refreshToken, string role, string accountId)> Login(string email, string password)
        {
            var (accessToken, refreshToken, role, accountId) = await _accountDAO.Login(email, password);
            return (accessToken, refreshToken, role, accountId);
        }

        public async Task<string> Register(RegisterRequest registerRequest)
        {
            return await _accountDAO.Register(registerRequest);
        }

        public async Task<string> RefreshAccessToken()
        {
            return await _accountDAO.RefreshAccessToken();
        }

        public void Logout()
        {
            _accountDAO.Logout();
        }
    }
}
