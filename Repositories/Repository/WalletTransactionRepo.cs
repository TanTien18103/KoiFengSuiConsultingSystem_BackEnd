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
        private readonly WalletTransactionDAO _walletTransactionDAO;

        public WalletTransactionRepo(WalletTransactionDAO walletTransactionDAO)
        {
            _walletTransactionDAO = walletTransactionDAO;
        }

        public async Task<WalletTransaction> GetWalletTransactionById(string walletTransactionId)
        {
            return await _walletTransactionDAO.GetWalletTransactionById(walletTransactionId);
        }

        public async Task<WalletTransaction> CreateWalletTransaction(WalletTransaction walletTransaction)
        {
            return await _walletTransactionDAO.CreateWalletTransaction(walletTransaction);
        }

        public async Task<WalletTransaction> UpdateWalletTransaction(WalletTransaction walletTransaction)
        {
            return await _walletTransactionDAO.UpdateWalletTransaction(walletTransaction);
        }

        public async Task DeleteWalletTransaction(string walletTransactionId)
        {
            await _walletTransactionDAO.DeleteWalletTransaction(walletTransactionId);
        }

        public async Task<List<WalletTransaction>> GetWalletTransactions()
        {
            return await _walletTransactionDAO.GetWalletTransactions();
        }
    }
}
