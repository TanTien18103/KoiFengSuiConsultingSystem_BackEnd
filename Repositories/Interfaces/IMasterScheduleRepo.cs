using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IMasterScheduleRepo
    {
        Task<MasterSchedule> GetMasterScheduleById(string masterScheduleId);
        Task<MasterSchedule> CreateMasterSchedule(MasterSchedule masterSchedule);
        Task<MasterSchedule> UpdateMasterSchedule(MasterSchedule masterSchedule);
        Task DeleteMasterSchedule(string masterScheduleId);
        Task<List<MasterSchedule>> GetAllSchedules();
        Task<List<MasterSchedule>> GetSchedulesByMasterId(string masterId);
        Task<List<MasterSchedule>> GetSchedulesByMasterAndDate(string masterId, DateOnly date);
    }
}
