using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class RegisterAttendDAO
    {
        public static RegisterAttendDAO instance = null;
        private readonly KoiFishPondContext _context;

        public RegisterAttendDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static RegisterAttendDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RegisterAttendDAO();
                }
                return instance;
            }
        }

        public async Task<RegisterAttend> GetRegisterAttendByIdDao(string registerAttendId)
        {
            return await _context.RegisterAttends.FindAsync(registerAttendId);
        }

        public async Task<List<RegisterAttend>> GetRegisterAttendsDao()
        {
            return await _context.RegisterAttends
                .Include(x => x.Workshop)
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .ToListAsync();
        }

        public async Task<RegisterAttend> CreateRegisterAttendDao(RegisterAttend registerAttend)
        {
            _context.RegisterAttends.Add(registerAttend);
            await _context.SaveChangesAsync();
            return registerAttend;
        }

        public async Task<RegisterAttend> UpdateRegisterAttendDao(RegisterAttend registerAttend)
        {
            _context.RegisterAttends.Update(registerAttend);
            await _context.SaveChangesAsync();
            return registerAttend;
        }

        public async Task DeleteRegisterAttendDao(string registerAttendId)
        {
            var registerAttend = await GetRegisterAttendByIdDao(registerAttendId);
            _context.RegisterAttends.Remove(registerAttend);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RegisterAttend>> GetRegisterAttendsByCustomerIdDao(string customerId)
        {
            return await _context.RegisterAttends
                .Include(x => x.Workshop)
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .Where(x => x.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<string> GetCustomerIdByAccountIdDao(string accountId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.AccountId == accountId);
            return customer.CustomerId;
        }

        public async Task<List<RegisterAttend>> GetRegisterAttendsByWorkshopIdDao(string workshopId)
        {
            return await _context.RegisterAttends
                .Include(x => x.Workshop)
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .Where(x => x.WorkshopId == workshopId)
                .ToListAsync();
        }
    }
}
