using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class TransactionDAO
    {
        private readonly KoiFishPondContext _context;

        public TransactionDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Transaction> GetTransactionById(string transactionId)
        {
            return await _context.Transactions.FindAsync(transactionId);
        }

        public async Task<List<Transaction>> GetTransactions()
        {
            return _context.Transactions.ToList();
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateTransaction(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }


        public async Task DeleteTransaction(string transactionId)
        {
            var transaction = await GetTransactionById(transactionId);
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
         

    }
}
