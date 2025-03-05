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
    public class WalletTransactionRepo : IWalletTransactionRepo
    {
        public Task<WalletTransaction> GetWalletTransactionById(string walletTransactionId)
        {
            return WalletTransactionDAO.Instance.GetWalletTransactionByIdDao(walletTransactionId);
        }

        public Task<WalletTransaction> CreateWalletTransaction(WalletTransaction walletTransaction)
        {
            return WalletTransactionDAO.Instance.CreateWalletTransactionDao(walletTransaction);
        }

        public Task<WalletTransaction> UpdateWalletTransaction(WalletTransaction walletTransaction)
        {
            return WalletTransactionDAO.Instance.UpdateWalletTransactionDao(walletTransaction);
        }

        public Task DeleteWalletTransaction(string walletTransactionId)
        {
            return WalletTransactionDAO.Instance.DeleteWalletTransactionDao(walletTransactionId);
        }

        public Task<List<WalletTransaction>> GetWalletTransactions()
        {
            return WalletTransactionDAO.Instance.GetWalletTransactionsDao();
        }
    }
}
