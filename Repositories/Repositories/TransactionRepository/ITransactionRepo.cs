using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.TransactionRepository
{
    public interface ITransactionRepo
    {
        Task<Transaction> GetTransactionById(string transactionId);
        Task<List<Transaction>> GetTransactions();
        Task<Transaction> CreateTransaction(Transaction transaction);
        Task<Transaction> UpdateTransaction(Transaction transaction);
        Task DeleteTransaction(string transactionId);
    }
}
