using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class MasterScheduleDAO
    {
        private static volatile MasterScheduleDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private MasterScheduleDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static MasterScheduleDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new MasterScheduleDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<MasterSchedule> GetMasterScheduleByIdDao(string masterScheduleId)
        {
            return await _context.MasterSchedules.FindAsync(masterScheduleId);
        }

        public async Task<List<MasterSchedule>> GetMasterSchedulesDao()
        {
            return _context.MasterSchedules.ToList();
        }

        public async Task<List<MasterSchedule>> GetAllSchedulesAsync()
        {
            return await _context.MasterSchedules
                .Include(x => x.Master)
                .Include(x => x.BookingOnlines)
                    .ThenInclude(b => b.Customer)
                        .ThenInclude(c => c.Account)
                .Include(x => x.BookingOfflines)
                    .ThenInclude(b => b.Customer)
                        .ThenInclude(c => c.Account)
                .OrderBy(x => x.Date)
                .ThenBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<List<MasterSchedule>> GetSchedulesByMasterIdAsync(string masterId)
        {
            return await _context.MasterSchedules
                .Include(x => x.Master)
                .Include(x => x.BookingOnlines)
                    .ThenInclude(b => b.Customer)
                        .ThenInclude(c => c.Account)
                .Include(x => x.BookingOfflines)
                    .ThenInclude(b => b.Customer)
                        .ThenInclude(c => c.Account)
                .Where(x => x.MasterId == masterId)
                .OrderBy(x => x.Date)
                .ThenBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<List<MasterSchedule>> GetSchedulesByMasterAndDateAsync(string masterId, DateOnly date)
        {
            return await _context.MasterSchedules
                .Include(x => x.Master)
                .Include(x => x.BookingOnlines)
                    .ThenInclude(b => b.Customer)
                        .ThenInclude(c => c.Account)
                .Include(x => x.BookingOfflines)
                    .ThenInclude(b => b.Customer)
                        .ThenInclude(c => c.Account)
                .Where(x => x.MasterId == masterId)
                .OrderBy(x => x.Date)
                .ThenBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<MasterSchedule> GetMasterScheduleDao(string masterId, DateOnly date, TimeOnly startTime)
        {
            return await _context.MasterSchedules
                .FirstOrDefaultAsync(ms => ms.MasterId == masterId && ms.Date == date && ms.StartTime == startTime);
        }

        public async Task<MasterSchedule> CreateMasterScheduleDao(MasterSchedule masterSchedule)
        {
            _context.MasterSchedules.Add(masterSchedule);
            await _context.SaveChangesAsync();
            return masterSchedule;
        }

        public async Task<MasterSchedule> UpdateMasterScheduleDao(MasterSchedule masterSchedule)
        {
            _context.MasterSchedules.Update(masterSchedule);
            await _context.SaveChangesAsync();
            return masterSchedule;
        }

        public async Task DeleteMasterScheduleDao(string masterScheduleId)
        {
            var masterSchedule = await GetMasterScheduleByIdDao(masterScheduleId);
            _context.MasterSchedules.Remove(masterSchedule);
            await _context.SaveChangesAsync();
        }
    }
}
