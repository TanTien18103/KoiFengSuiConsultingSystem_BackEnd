using BusinessObjects.Models;
using DAOs.DAOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class MasterScheduleRepo : IMasterScheduleRepo
    {
        private readonly MasterScheduleDAO _masterScheduleDAO;

        public MasterScheduleRepo(MasterScheduleDAO masterScheduleDAO)
        {
            _masterScheduleDAO = masterScheduleDAO;
        }

        public async Task<MasterSchedule> GetMasterScheduleById(string masterScheduleId)
        {
            return await _masterScheduleDAO.GetMasterScheduleById(masterScheduleId);
        }

        public async Task<MasterSchedule> CreateMasterSchedule(MasterSchedule masterSchedule)
        {
            return await _masterScheduleDAO.CreateMasterSchedule(masterSchedule);
        }

        public async Task<MasterSchedule> UpdateMasterSchedule(MasterSchedule masterSchedule)
        {
            return await _masterScheduleDAO.UpdateMasterSchedule(masterSchedule);
        }

        public async Task DeleteMasterSchedule(string masterScheduleId)
        {
            await _masterScheduleDAO.DeleteMasterSchedule(masterScheduleId);
        }

        public async Task<List<MasterSchedule>> GetMasterSchedules()
        {
            return await _masterScheduleDAO.GetMasterSchedules();
        }
    }
}
