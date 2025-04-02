using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class AccountDAO
    {
        private static volatile AccountDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private AccountDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static AccountDAO Instance
        {
            get
            {
                if (_instance == null) 
                {
                    lock (_lock) 
                    {
                        if (_instance == null) 
                        {
                            _instance = new AccountDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Account> GetAccountByUniqueFieldsDao(string userName, string email, string phoneNumber, int bankId, string accountNo, string currentAccountId)
        {
            return await _context.Accounts
                .Where(a => (a.UserName == userName || a.Email == email || a.PhoneNumber == phoneNumber ||
                             (a.BankId == bankId && a.AccountNo == accountNo))
                            && a.AccountId != currentAccountId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Account>> GetAccountsByIds(List<string> accountIds)
        {
            return await _context.Accounts
                .Where(a => accountIds.Contains(a.AccountId))
                .ToListAsync();
        }

        public async Task<List<Account>> GetAllStaffDao()
        {
            return await _context.Accounts
               .Where(a => a.Role == "Staff")
               .ToListAsync();
        }

        public async Task<Account?> GetAccountByEmailDao(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Account?> GetAccountByIdDao(string accountId)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == accountId);
        }

        public async Task<string> GetAccountIdFromTokenDao(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var accountIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                return accountIdClaim?.Value ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task AddAccountDao(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task<Account> UpdateAccountDao(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<List<Account>> GetAllAccountsDao(string? role = null)
        {
            var query = _context.Accounts.AsQueryable();
            
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(a => a.Role == role);
            }
            
            return await query.ToListAsync();
        }

        public async Task<List<Account>> GetAccountsByRoleDao(string role)
        {
            return await _context.Accounts
                .Where(a => a.Role == role)
                .ToListAsync();
        }

        public async Task<Account> ToggleAccountStatusDao(string accountId, bool isActive)
        {
            var account = await GetAccountByIdDao(accountId);
            if (account != null)
            {
                account.IsActive = isActive;
                await _context.SaveChangesAsync();
            }
            return account;
        }

        public async Task DeleteAccountDao(string accountId)
        {
            var account = await GetAccountByIdDao(accountId);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Account> UpdateAccountRoleDao(string accountId, string newRole)
        {
            var account = await GetAccountByIdDao(accountId);
            if (account != null)
            {
                account.Role = newRole;
                await _context.SaveChangesAsync();
            }
            return account;
        }
    }
}
