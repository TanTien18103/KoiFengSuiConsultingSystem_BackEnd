using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class WalletDAO
    {
        private readonly KoiFishPondContext _context;

        public WalletDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Wallet> GetWalletById(string walletId)
        {
            return await _context.Wallets.FindAsync(walletId);
        }

        public async Task<List<Wallet>> GetWallets()
        {
            return _context.Wallets.ToList();
        }

        public async Task<Wallet> CreateWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet> UpdateWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task DeleteWallet(string walletId)
        {
            var wallet = await GetWalletById(walletId);
            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
        }
    }
}
