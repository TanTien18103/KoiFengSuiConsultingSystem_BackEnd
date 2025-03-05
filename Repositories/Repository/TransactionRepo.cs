using BusinessObjects.Models;
using DAOs.DAOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class TransactionRepo : ITransactionRepo
    {
        public Task<Transaction> GetTransactionById(string transactionId)
        {
            return TransactionDAO.Instance.GetTransactionByIdDao(transactionId);
        }

        public Task<Transaction> CreateTransaction(Transaction transaction)
        {
            return TransactionDAO.Instance.CreateTransactionDao(transaction);
        }

        public Task<Transaction> UpdateTransaction(Transaction transaction)
        {
            return TransactionDAO.Instance.UpdateTransactionDao(transaction);
        }

        public Task DeleteTransaction(string transactionId)
        {
            return TransactionDAO.Instance.DeleteTransactionDao(transactionId);
        }

        public Task<List<Transaction>> GetTransactions()
        {
            return TransactionDAO.Instance.GetTransactionsDao();
        }
    }
}
