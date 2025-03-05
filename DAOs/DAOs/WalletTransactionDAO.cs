using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class WalletTransactionDAO
    {
        public static WalletTransactionDAO instance = null;
        private readonly KoiFishPondContext _context;

        public WalletTransactionDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static WalletTransactionDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WalletTransactionDAO();
                }
                return instance;
            }
        }

        public async Task<WalletTransaction> GetWalletTransactionByIdDao(string walletTransactionId)
        {
            return await _context.WalletTransactions.FindAsync(walletTransactionId);
        }

        public async Task<List<WalletTransaction>> GetWalletTransactionsDao()
        {
            return _context.WalletTransactions.ToList();
        }

        public async Task<WalletTransaction> CreateWalletTransactionDao(WalletTransaction walletTransaction)
        {
            _context.WalletTransactions.Add(walletTransaction);
            await _context.SaveChangesAsync();
            return walletTransaction;
        }

        public async Task<WalletTransaction> UpdateWalletTransactionDao(WalletTransaction walletTransaction)
        {
            _context.WalletTransactions.Update(walletTransaction);
            await _context.SaveChangesAsync();
            return walletTransaction;
        }

        public async Task DeleteWalletTransactionDao(string walletTransactionId)
        {
            var walletTransaction = await GetWalletTransactionByIdDao(walletTransactionId);
            _context.WalletTransactions.Remove(walletTransaction);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WalletTransaction>> GetWalletTransactionsByWalletIdDao(string walletId)
        {
            return _context.WalletTransactions.Where(wt => wt.WalletId == walletId).ToList();
        }
    }
}
