using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class ContractDAO
    {
        public static ContractDAO instance = null;
        private readonly KoiFishPondContext _context;

        public ContractDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static ContractDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ContractDAO();
                }
                return instance;
            }
        }

        public async Task<Contract> GetContractByIdDao(string contractId)
        {
            return await _context.Contracts.FindAsync(contractId);
        }

        public async Task<List<Contract>> GetContractsDao()
        {
            return _context.Contracts.ToList();
        }

        public async Task<Contract> CreateContractDao(Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<Contract> UpdateContractDao(Contract contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task DeleteContractDao(string contractId)
        {
            var contract = await GetContractByIdDao(contractId);
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
        }

    }
}
