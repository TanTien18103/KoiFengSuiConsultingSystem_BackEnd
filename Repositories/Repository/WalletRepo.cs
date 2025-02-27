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
        private readonly WalletDAO _walletDAO;

        public WalletRepo(WalletDAO walletDAO)
        {
            _walletDAO = walletDAO;
        }

        public async Task<Wallet> GetWalletById(string walletId)
        {
            return await _walletDAO.GetWalletById(walletId);
        }

        public async Task<Wallet> CreateWallet(Wallet wallet)
        {
            return await _walletDAO.CreateWallet(wallet);
        }

        public async Task<Wallet> UpdateWallet(Wallet wallet)
        {
            return await _walletDAO.UpdateWallet(wallet);
        }

        public async Task DeleteWallet(string walletId)
        {
            await _walletDAO.DeleteWallet(walletId);
        }

        public async Task<List<Wallet>> GetWallets()
        {
            return await _walletDAO.GetWallets();
        }
    }
}
