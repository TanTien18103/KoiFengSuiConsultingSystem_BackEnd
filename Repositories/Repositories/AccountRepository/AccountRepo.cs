using BusinessObjects.Models;
using DAOs.DAOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.AccountRepository
{
    public class AccountRepo : IAccountRepo
    {
        public Task<Account?> GetAccountByEmail(string email)
        {
            return AccountDAO.Instance.GetAccountByEmailDao(email);
        }

        public Task<Account> GetAccountById(string accountId)
        {
            return AccountDAO.Instance.GetAccountByIdDao(accountId);
        }

        public Task<string> GetAccountIdFromToken(string token)
        {
            return AccountDAO.Instance.GetAccountIdFromTokenDao(token);
        }

        public Task AddAccount(Account account)
        {
            return AccountDAO.Instance.AddAccountDao(account);
        }

        public Task<Account> UpdateAccount(Account account)
        {
            return AccountDAO.Instance.UpdateAccountDao(account);
        }
    }
}
