using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IContractRepo
    {
        Task<Contract> GetContractById(string contractId);
        Task<List<Contract>> GetContracts();
        Task<Contract> CreateContract(Contract contract);
        Task<Contract> UpdateContract(Contract contract);
        Task DeleteContract(string contractId);
    }
}
