using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class MasterScheduleDAO
    {
        private readonly KoiFishPondContext _context;

        public MasterScheduleDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<MasterSchedule> GetMasterScheduleById(string masterScheduleId)
        {
            return await _context.MasterSchedules.FindAsync(masterScheduleId);
        }

        public async Task<List<MasterSchedule>> GetMasterSchedules()
        {
            return _context.MasterSchedules.ToList();
        }

        public async Task<MasterSchedule> CreateMasterSchedule(MasterSchedule masterSchedule)
        {
            _context.MasterSchedules.Add(masterSchedule);
            await _context.SaveChangesAsync();
            return masterSchedule;
        }

        public async Task<MasterSchedule> UpdateMasterSchedule(MasterSchedule masterSchedule)
        {
            _context.MasterSchedules.Update(masterSchedule);
            await _context.SaveChangesAsync();
            return masterSchedule;
        }

        public async Task DeleteMasterSchedule(string masterScheduleId)
        {
            var masterSchedule = await GetMasterScheduleById(masterScheduleId);
            _context.MasterSchedules.Remove(masterSchedule);
            await _context.SaveChangesAsync();
        }

    }
}
