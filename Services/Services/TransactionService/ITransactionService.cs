using BusinessObjects.Models;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.TransactionService
{
    public interface ITransactionService
    {
        Task<ResultModel> GetTransactionById(string transactionId);
        Task<ResultModel> GetTransactions();
        Task<ResultModel> CreateTransaction(Transaction transaction);
        Task<ResultModel> UpdateTransaction(Transaction transaction);
        Task<ResultModel> DeleteTransaction(string transactionId);
        Task<ResultModel> CreateTransactionWithDocNo(string docNo, string method, string type);
    }
}

