using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.AccountRepository
{
    public interface IAccountRepo
    {
        Task<Account?> GetAccountByEmail(string email);
        Task<string> GetAccountIdFromToken(string token);
        Task<Account> GetAccountById(string accountId);
        Task AddAccount(Account account);
        Task<Account> UpdateAccount(Account account);
    }
}
