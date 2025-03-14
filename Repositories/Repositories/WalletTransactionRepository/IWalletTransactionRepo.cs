using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.WalletTransactionRepository
{
    public interface IWalletTransactionRepo
    {
        Task<WalletTransaction> GetWalletTransactionById(string walletTransactionId);
        Task<List<WalletTransaction>> GetWalletTransactions();
        Task<WalletTransaction> CreateWalletTransaction(WalletTransaction walletTransaction);
        Task<WalletTransaction> UpdateWalletTransaction(WalletTransaction walletTransaction);
        Task DeleteWalletTransaction(string walletTransactionId);
    }
}
