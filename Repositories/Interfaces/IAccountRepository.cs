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
        Task<string> GetAccount(string email, string password);
        Task<(string accessToken, string refreshToken)> Login(string email, string password);
        Task<string> Register(RegisterRequest registerRequest);
        Task<string> RefreshAccessToken();
        void Logout();
    }
}
