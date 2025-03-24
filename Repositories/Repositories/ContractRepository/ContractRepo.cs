using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.ContractRepository
{
    public class ContractRepo : IContractRepo
    {
        public Task<Contract> GetContractById(string contractId)
        {
            return ContractDAO.Instance.GetContractByIdDao(contractId);
        }
        public Task<List<Contract>> GetContracts()
        {
            return ContractDAO.Instance.GetContractsDao();
        }
        public Task<Contract> CreateContract(Contract contract)
        {
            return ContractDAO.Instance.CreateContractDao(contract);
        }
        public Task<Contract> UpdateContract(Contract contract)
        {
            return ContractDAO.Instance.UpdateContractDao(contract);
        }
        public Task DeleteContract(string contractId)
        {
            return ContractDAO.Instance.DeleteContractDao(contractId);
        }
        public Task<Contract> GetContractByBookingOfflineId(string bookingOfflineId)
        {
            return ContractDAO.Instance.GetContractByBookingOfflineIdDao(bookingOfflineId);
        }
    }
}
