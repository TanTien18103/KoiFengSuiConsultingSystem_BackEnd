using BusinessObjects.Models;
using DAOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountByEmailAsync(string email);
        Task AddAccountAsync(Account account);
    }
}
