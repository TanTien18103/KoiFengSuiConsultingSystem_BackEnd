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
        public Task<MasterSchedule> GetMasterScheduleById(string masterScheduleId)
        {
            return MasterScheduleDAO.Instance.GetMasterScheduleByIdDao(masterScheduleId);
        }

        public Task<MasterSchedule> CreateMasterSchedule(MasterSchedule masterSchedule)
        {
            return MasterScheduleDAO.Instance.CreateMasterScheduleDao(masterSchedule);
        }

        public Task<MasterSchedule> UpdateMasterSchedule(MasterSchedule masterSchedule)
        {
            return MasterScheduleDAO.Instance.UpdateMasterScheduleDao(masterSchedule);
        }

        public Task DeleteMasterSchedule(string masterScheduleId)
        {
            return MasterScheduleDAO.Instance.DeleteMasterScheduleDao(masterScheduleId);
        }

        public Task<List<MasterSchedule>> GetMasterSchedules()
        {
            return MasterScheduleDAO.Instance.GetMasterSchedulesDao();
        }
    }
}
