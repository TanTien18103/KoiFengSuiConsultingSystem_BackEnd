using BusinessObjects.Constants;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.TransactionRepository;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.TransactionService
{
    public class TransactionService : ITransactionService
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
            var res = new ResultModel();
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
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Data = transactionId;
                res.Message = ResponseMessageConstrantsTransaction.TRANSACTION_CREATED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Transaction creation failed: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> CreateTransaction(Transaction transaction)
        {
            var res = new ResultModel();
            try
            {
                var transactionId = await _transactionRepository.CreateTransaction(transaction);
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Data = transactionId;
                res.Message = ResponseMessageConstrantsTransaction.TRANSACTION_CREATED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Transaction creation failed: {ex.Message}";
                return res;
            }
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
