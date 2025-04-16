using Services.ApiModels;
using Services.ApiModels.Account;
using Services.ApiModels.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AccountService
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
        Task<ResultModel> ChangePassword(ChangePasswordRequest request);
        Task<ResultModel> ForgotPassword(string email);

        // Account Management (Admin only)
        Task<ResultModel> GetAllAccounts(string? role = null);
        Task<ResultModel> GetAccountsByRole(string role);
        Task<ResultModel> ToggleAccountStatus(string accountId, bool isActive);
        Task<ResultModel> DeleteAccount(string accountId);
        Task<ResultModel> UpdateAccountRole(string accountId, string newRole);
        Task<ResultModel> GetAllStaff();
        Task<ResultModel> GetAllCustomers();

        //Master
        Task<ResultModel> UpdateMasterProfile( MasterRequest request);
    }
}
