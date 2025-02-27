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
        private readonly KoiFishPondContext _context;

        public ContractDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Contract> GetContractById(string contractId)
        {
            return await _context.Contracts.FindAsync(contractId);
        }

        public async Task<List<Contract>> GetContracts()
        {
            return _context.Contracts.ToList();
        }

        public async Task<Contract> CreateContract(Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<Contract> UpdateContract(Contract contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task DeleteContract(string contractId)
        {
            var contract = await GetContractById(contractId);
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
        }

    }
}
