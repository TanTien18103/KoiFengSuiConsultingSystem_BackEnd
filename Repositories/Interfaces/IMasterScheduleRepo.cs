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
        Task<List<MasterSchedule>> GetMasterSchedules();
        Task<MasterSchedule> CreateMasterSchedule(MasterSchedule masterSchedule);
        Task<MasterSchedule> UpdateMasterSchedule(MasterSchedule masterSchedule);
        Task DeleteMasterSchedule(string masterScheduleId);
    }
}
