using DAOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<string> RegisterGoogleUser(string name, string email);
        Task<string> GetAccountIdFromToken(string token);
        Task<string> GetAccount(string email, string password);
        Task<(string accessToken, string refreshToken, string role, string accountId)> Login(string email, string password);
        Task<string> Register(RegisterRequest registerRequest);
        Task<string> RefreshAccessToken();
        void Logout();
    }
}
