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
        public static WalletDAO instance = null;
        private readonly KoiFishPondContext _context;

        public WalletDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static WalletDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WalletDAO();
                }
                return instance;
            }
        }

        public async Task<Wallet> GetWalletByIdDao(string walletId)
        {
            return await _context.Wallets.FindAsync(walletId);
        }

        public async Task<List<Wallet>> GetWalletsDao()
        {
            return _context.Wallets.ToList();
        }

        public async Task<Wallet> CreateWalletDao(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet> UpdateWalletDao(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task DeleteWalletDao(string walletId)
        {
            var wallet = await GetWalletByIdDao(walletId);
            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
        }
    }
}
