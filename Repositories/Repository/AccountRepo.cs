using BusinessObjects.Models;
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
    public class AccountRepo : IAccountRepo
    {
        public Task AddAccount(Account account)
        {
            return AccountDAO.Instance.AddAccountDao(account);
        }

        public Task<Account?> GetAccountByEmail(string email)
        {
            return AccountDAO.Instance.GetAccountByEmailDao(email);
        }

        public Task<string> GetAccountIdFromToken(string token)
        {
            return AccountDAO.Instance.GetAccountIdFromTokenDao(token);
        }

        public Task<Account> UpdateAccount(Account account)
        {
            return AccountDAO.Instance.UpdateAccountDao(account);
        }
    }
}
