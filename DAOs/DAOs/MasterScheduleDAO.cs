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
        public static MasterScheduleDAO instance = null;
        private readonly KoiFishPondContext _context;

        public MasterScheduleDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static MasterScheduleDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MasterScheduleDAO();
                }
                return instance;
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
