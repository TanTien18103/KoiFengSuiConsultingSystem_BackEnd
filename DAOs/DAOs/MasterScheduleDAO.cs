﻿using BusinessObjects.Enums;
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
            return await _context.MasterSchedules.ToListAsync();
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
                .AsNoTracking()
                .Include(ms => ms.Master)
                .Include(ms => ms.BookingOnlines)
                    .ThenInclude(bo => bo.Customer)
                        .ThenInclude(c => c.Account)
                .Include(ms => ms.BookingOfflines)
                    .ThenInclude(boff => boff.Customer)
                        .ThenInclude(c => c.Account)
                .Include(x => x.WorkShops) 
                        .ThenInclude(w => w.Location)
                .Where(x => x.MasterId == masterId && x.Status == MasterScheduleEnums.InProgress.ToString())
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

        public async Task<MasterSchedule> GetMasterScheduleByMasterIdAndWorkshopIdDao(string masterId, string workshopId)
        {
            var workshop = await _context.WorkShops
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.WorkshopId == workshopId);

            if (workshop == null || string.IsNullOrEmpty(workshop.MasterId))
            {
                return null;
            }
            return await _context.MasterSchedules
                .Include(x => x.Master)
                .Include(x => x.BookingOnlines)
                    .ThenInclude(b => b.Customer)
                        .ThenInclude(c => c.Account)
                .Include(x => x.BookingOfflines)
                    .ThenInclude(b => b.Customer)
                        .ThenInclude(c => c.Account)
                .FirstOrDefaultAsync(ms => ms.MasterId == masterId && workshop.WorkshopId == workshopId);
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

        public async Task<List<MasterSchedule>> GetMasterScheduleByMasterId(string masterId)
        {
            var masterSchedules = await _context.MasterSchedules
                .Where(x => x.MasterId == masterId)
                .ToListAsync();
            return masterSchedules;
        }

        public async Task<bool> CheckMasterScheduleAvailabilityDao(string masterId, DateOnly? bookingDate, TimeOnly? startTime, TimeOnly? endTime)
        {
            if (!bookingDate.HasValue || !startTime.HasValue || !endTime.HasValue)
                return false;

            return await _context.MasterSchedules
                .AnyAsync(s => s.MasterId == masterId &&
                             s.Date == bookingDate &&
                             ((s.StartTime <= startTime && s.EndTime > startTime) ||
                              (s.StartTime < endTime && s.EndTime >= endTime) ||
                              (s.StartTime >= startTime && s.EndTime <= endTime)));
        }

        public async Task<MasterSchedule> GetMasterScheduleByDateAndTimeDao(DateOnly bookingDate, TimeOnly startTime, string masterId)
        {
            return await _context.MasterSchedules
                .FirstOrDefaultAsync(ms => 
                    ms.Date == bookingDate && 
                    ms.StartTime == startTime && 
                    ms.MasterId == masterId);
        }

        public async Task<MasterSchedule> GetMasterScheduleByMasterIdAndStartTimeEndTimeAndDate(string masterId, TimeOnly startTime, TimeOnly endTime, DateOnly date)
        {
            return await _context.MasterSchedules
                .FirstOrDefaultAsync(ms =>
                    ms.MasterId == masterId &&
                    ms.StartTime == startTime &&
                    ms.EndTime == endTime &&
                    ms.Date == date);
        }

        public async Task<List<MasterSchedule>> GetMasterScheduleByMasterIdAndDateDao(string masterId, DateOnly dateOnly)
        {
            return await _context.MasterSchedules
                .Where(ms => ms.MasterId == masterId && ms.Date == dateOnly)
                .ToListAsync();
        }
    }
}
