using BusinessObjects.Models;
using DAOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IAccountRepo
    {
        Task AddAccount(Account account);
        Task<Account?> GetAccountByEmail(string email);
        Task<string> GetAccountIdFromToken(string token);
        Task<Account> UpdateAccount(Account account);
    }
}
