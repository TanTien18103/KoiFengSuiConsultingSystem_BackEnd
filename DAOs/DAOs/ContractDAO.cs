using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class ContractDAO
    {
        private static volatile ContractDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private ContractDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static ContractDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ContractDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Contract> GetContractByIdDao(string contractId)
        {
            return await _context.Contracts.Include(c => c.BookingOfflines).ThenInclude(b => b.Customer).ThenInclude(c => c.Account).FirstOrDefaultAsync(c => c.ContractId == contractId);
        }
        public async Task<Contract> GetContractByBookingOfflineIdDao(string bookingOfflineId)
        {
            return await _context.Contracts.Include(c => c.BookingOfflines).FirstOrDefaultAsync(c => c.BookingOfflines.Any(b => b.BookingOfflineId == bookingOfflineId));
        }
        public async Task<List<Contract>> GetContractsDao()
        {
            return await _context.Contracts
                .Include(c => c.BookingOfflines)
                .ToListAsync();
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
