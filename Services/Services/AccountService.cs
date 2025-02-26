using DAOs.Request;
using Repositories.Interfaces;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<string> GetAccount(string email, string password)
        {
            return await _accountRepository.GetAccount(email, password);
        }

        public async Task<(string accessToken, string refreshToken)> Login(string email, string password)
        {
            return await _accountRepository.Login(email, password);
        }

        public async Task<string> Register(RegisterRequest registerRequest)
        {
            return await _accountRepository.Register(registerRequest);
        }

        public async Task<string> RefreshAccessToken()
        {
            return await _accountRepository.RefreshAccessToken();
        }

        public void Logout()
        {
            _accountRepository.Logout();
        }
        public async Task<string> RegisterGoogleUser(string name, string email)
        {
            return await _accountRepository.RegisterGoogleUser(name, email);
        }
    }
}
