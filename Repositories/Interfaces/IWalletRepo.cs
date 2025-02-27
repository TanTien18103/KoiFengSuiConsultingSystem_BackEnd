using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IWalletRepo
    {
        Task<Wallet> GetWalletById(string walletId);
        Task<List<Wallet>> GetWallets();
        Task<Wallet> CreateWallet(Wallet wallet);
        Task<Wallet> UpdateWallet(Wallet wallet);
        Task DeleteWallet(string walletId);
    }
}
