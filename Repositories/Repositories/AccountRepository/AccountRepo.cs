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
        public Task<List<Account>> GetAllAccounts(string? role = null)
        {
            return AccountDAO.Instance.GetAllAccountsDao(role);
        }

        public Task<List<Account>> GetAccountsByRole(string role)
        {
            return AccountDAO.Instance.GetAccountsByRoleDao(role);
        }

        public Task<Account> ToggleAccountStatus(string accountId, bool isActive)
        {
            return AccountDAO.Instance.ToggleAccountStatusDao(accountId, isActive);
        }

        public Task DeleteAccount(string accountId)
        {
            return AccountDAO.Instance.DeleteAccountDao(accountId);
        }

        public Task<Account> UpdateAccountRole(string accountId, string newRole)
        {
            return AccountDAO.Instance.UpdateAccountRoleDao(accountId, newRole);
        }
    }
}
