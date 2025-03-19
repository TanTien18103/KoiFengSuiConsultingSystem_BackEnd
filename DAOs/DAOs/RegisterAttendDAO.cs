using BusinessObjects.Enums;
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
        private static volatile RegisterAttendDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private RegisterAttendDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static RegisterAttendDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new RegisterAttendDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<RegisterAttend> GetRegisterAttendByIdDao(string registerAttendId)
        {
            return await _context.RegisterAttends
                .Include(x => x.Workshop)
                .Include(x => x.Customer)
                    .ThenInclude(c => c.Account)
                .FirstOrDefaultAsync(x => x.AttendId == registerAttendId);
        }

        public async Task<List<RegisterAttend>> GetRegisterAttendsDao()
        {
            return await _context.RegisterAttends
                .Include(x => x.Workshop)
                .Include(x => x.Customer)
                    .ThenInclude(c => c.Account)
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

        public async Task<List<RegisterAttend>> GetRegisterAttendsByGroupIdDao(string groupId)
        {
            return await _context.RegisterAttends
                .Include(x => x.Workshop)
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .Where(x => x.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<List<RegisterAttend>> GetPendingTicketsDao(string workshopId, string customerId)
        {
            return await _context.RegisterAttends
                .AsNoTracking() 
                .Where(x => 
                    x.WorkshopId == workshopId && 
                    x.CustomerId == customerId && 
                    x.Status == RegisterAttendStatusEnums.Pending.ToString())
                .ToListAsync();
        }
    }
}
