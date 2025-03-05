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
        public static TransactionDAO instance = null;
        private readonly KoiFishPondContext _context;

        public TransactionDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static TransactionDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TransactionDAO();
                }
                return instance;
            }
        }

        public async Task<Transaction> GetTransactionByIdDao(string transactionId)
        {
            return await _context.Transactions.FindAsync(transactionId);
        }

        public async Task<List<Transaction>> GetTransactionsDao()
        {
            return _context.Transactions.ToList();
        }

        public async Task<Transaction> CreateTransactionDao(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateTransactionDao(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }


        public async Task DeleteTransactionDao(string transactionId)
        {
            var transaction = await GetTransactionByIdDao(transactionId);
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
         

    }
}
