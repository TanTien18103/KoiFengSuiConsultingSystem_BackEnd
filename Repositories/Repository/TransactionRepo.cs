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
        private readonly TransactionDAO _transactionDAO;

        public TransactionRepo(TransactionDAO transactionDAO)
        {
            _transactionDAO = transactionDAO;
        }

        public async Task<Transaction> GetTransactionById(string transactionId)
        {
            return await _transactionDAO.GetTransactionById(transactionId);
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction)
        {
            return await _transactionDAO.CreateTransaction(transaction);
        }

        public async Task<Transaction> UpdateTransaction(Transaction transaction)
        {
            return await _transactionDAO.UpdateTransaction(transaction);
        }

        public async Task DeleteTransaction(string transactionId)
        {
            await _transactionDAO.DeleteTransaction(transactionId);
        }

        public async Task<List<Transaction>> GetTransactions()
        {
            return await _transactionDAO.GetTransactions();
        }
    }
}
