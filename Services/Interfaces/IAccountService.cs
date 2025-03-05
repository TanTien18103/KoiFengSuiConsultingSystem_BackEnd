using Services.ApiModels;
using Services.ApiModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAccountService
    {
        Task<string> RegisterGoogleUser(string name, string email);
        Task<string> GetAccount(string email, string password);
        Task<(string accessToken, string refreshToken)> Login(string email, string password);
        Task<string> Register(RegisterRequest registerRequest);
        Task<string> RefreshAccessToken();
        void Logout();
        Task<ResultModel> EditProfile(EditProfileRequest request);
    }
}
