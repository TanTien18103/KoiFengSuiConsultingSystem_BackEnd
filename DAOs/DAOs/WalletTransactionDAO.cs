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
        private readonly KoiFishPondContext _context;


        public WalletTransactionDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<WalletTransaction> GetWalletTransactionById(string walletTransactionId)
        {
            return await _context.WalletTransactions.FindAsync(walletTransactionId);
        }

        public async Task<List<WalletTransaction>> GetWalletTransactions()
        {
            return _context.WalletTransactions.ToList();
        }

        public async Task<WalletTransaction> CreateWalletTransaction(WalletTransaction walletTransaction)
        {
            _context.WalletTransactions.Add(walletTransaction);
            await _context.SaveChangesAsync();
            return walletTransaction;
        }

        public async Task<WalletTransaction> UpdateWalletTransaction(WalletTransaction walletTransaction)
        {
            _context.WalletTransactions.Update(walletTransaction);
            await _context.SaveChangesAsync();
            return walletTransaction;
        }

        public async Task DeleteWalletTransaction(string walletTransactionId)
        {
            var walletTransaction = await GetWalletTransactionById(walletTransactionId);
            _context.WalletTransactions.Remove(walletTransaction);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WalletTransaction>> GetWalletTransactionsByWalletId(string walletId)
        {
            return _context.WalletTransactions.Where(wt => wt.WalletId == walletId).ToList();
        }
    }
}
