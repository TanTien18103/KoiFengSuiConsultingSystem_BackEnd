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
        Task<List<Account>> GetAccountsByIds(List<string> accountIds);
        Task<Account?> GetAccountByEmail(string email);
        Task<string> GetAccountIdFromToken(string token);
        Task<Account> GetAccountById(string accountId);
        Task<Account> GetAccountByUniqueFields(string userName, string email, string phoneNumber, int bankId, string accountNo, string currentAccountId);
        Task AddAccount(Account account);
        Task<Account> UpdateAccount(Account account);        
        Task<List<Account>> GetAllAccounts(string? role = null);
        Task<List<Account>> GetAccountsByRole(string role);
        Task<Account> ToggleAccountStatus(string accountId, bool isActive);
        Task DeleteAccount(string accountId);
        Task<Account> UpdateAccountRole(string accountId, string newRole);
    }
}
