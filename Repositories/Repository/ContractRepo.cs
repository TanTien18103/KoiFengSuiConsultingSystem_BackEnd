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
    public class ContractRepo : IContractRepo
    {
        private readonly ContractDAO _contractDAO;

        public ContractRepo(ContractDAO contractDAO)
        {
            _contractDAO = contractDAO;
        }

        public async Task<Contract> GetContractById(string contractId)
        {
            return await _contractDAO.GetContractById(contractId);
        }

        public async Task<Contract> CreateContract(Contract contract)
        {
            return await _contractDAO.CreateContract(contract);
        }

        public async Task<Contract> UpdateContract(Contract contract)
        {
            return await _contractDAO.UpdateContract(contract);
        }

        public async Task DeleteContract(string contractId)
        {
            await _contractDAO.DeleteContract(contractId);
        }

        public async Task<List<Contract>> GetContracts()
        {
            return await _contractDAO.GetContracts();
        }
    }
}
