using BusinessObjects.Models;
using DAOs.DAOs;
using DAOs.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDAO _accountDAO;


        public AccountRepository()
        {
            _accountDAO = new AccountDAO();
        }

        public async Task AddAccountAsync(Account account)
        {
            await _accountDAO.AddAccountAsync(account);
        }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            return await _accountDAO.GetAccountByEmailAsync(email);
        }
    }
}
