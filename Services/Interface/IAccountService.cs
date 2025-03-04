using DAOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAccountService
    {
        Task<string> RegisterGoogleUser(string name, string email);
        Task<string> GetAccount(string email, string password);
        Task<object> Login(string email, string password);
        Task<string> Register(RegisterRequest registerRequest);
        Task<string> RefreshAccessToken();
        void Logout();
    }
}
