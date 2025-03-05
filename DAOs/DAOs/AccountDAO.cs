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
        public static AccountDAO instance = null;
        private readonly KoiFishPondContext _context;

        public AccountDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static AccountDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountDAO();
                }
                return instance;
            }
        }

        public async Task<Account?> GetAccountByEmailDao(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task AddAccountDao(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
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

        public async Task<Account> UpdateAccountDao(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return account;
        }
    }
}
