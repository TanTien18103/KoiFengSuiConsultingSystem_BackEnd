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
    public class WalletRepo : IWalletRepo
    {
        public Task<Wallet> GetWalletById(string walletId)
        {
            return WalletDAO.Instance.GetWalletByIdDao(walletId);
        }

        public Task<Wallet> CreateWallet(Wallet wallet)
        {
            return WalletDAO.Instance.CreateWalletDao(wallet);
        }

        public Task<Wallet> UpdateWallet(Wallet wallet)
        {
            return WalletDAO.Instance.UpdateWalletDao(wallet);
        }

        public Task DeleteWallet(string walletId)
        {
            return WalletDAO.Instance.DeleteWalletDao(walletId);
        }

        public Task<List<Wallet>> GetWallets()
        {
            return WalletDAO.Instance.GetWalletsDao();
        }
    }
}
