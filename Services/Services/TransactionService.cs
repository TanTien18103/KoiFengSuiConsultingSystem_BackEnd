using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.ApiModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class TransactionService :  ITransactionService
    {
        private readonly ITransactionRepo _transactionRepository;

        public TransactionService(ITransactionRepo transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }

        public async Task<ResultModel> CreateTransactionWithDocNo(string docNo, string method, string type)
        {
            var result = new ResultModel();
            try
            {
                var newTransaction = new Transaction
                {
                    TransactionId = GenerateShortGuid(),
                    TransactionType = type,
                    DocNo = docNo, 
                    TransactionName = method,
                    Status = "Pending"
                };

                var transactionId = await _transactionRepository.CreateTransaction(newTransaction);
                result.Data = transactionId;
                result.Message = "Transaction created successfully";
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = $"Transaction creation failed: {ex.Message}";
                result.IsSuccess = false;
            }
            return result;
        }

        public async Task<ResultModel> CreateTransaction(Transaction transaction)
        {
            var result = new ResultModel();
            try
            {
                var transactionId = await _transactionRepository.CreateTransaction(transaction);
                result.Data = transactionId;
                result.Message = "Transaction created successfully";
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
            }
            return result;
        }

        public Task<ResultModel> DeleteTransaction(string transactionId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> GetTransactionById(string transactionId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> GetTransactions()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> UpdateTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
