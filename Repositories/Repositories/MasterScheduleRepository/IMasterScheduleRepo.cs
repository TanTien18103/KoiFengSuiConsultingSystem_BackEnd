using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.MasterScheduleRepository
{
    public interface IMasterScheduleRepo
    {
        Task<MasterSchedule> GetMasterScheduleById(string masterScheduleId);
        Task<List<MasterSchedule>> GetAllSchedules();
        Task<List<MasterSchedule>> GetSchedulesByMasterId(string masterId);
        Task<List<MasterSchedule>> GetSchedulesByMasterAndDate(string masterId, DateOnly date);
        Task<MasterSchedule> GetMasterSchedule(string masterId, DateOnly date, TimeOnly startTime);
        Task<MasterSchedule> CreateMasterSchedule(MasterSchedule masterSchedule);
        Task<MasterSchedule> UpdateMasterSchedule(MasterSchedule masterSchedule);
        Task<MasterSchedule> GetMasterScheduleByMasterIdAndWorkshopId(string masterId, string workshopId);
        Task DeleteMasterSchedule(string masterScheduleId);
        Task<List<MasterSchedule>> GetMasterScheduleByMasterId(string masterId);
        Task<bool> CheckMasterScheduleAvailabilityRepo(string masterId, DateOnly? bookingDate, TimeOnly? startTime, TimeOnly? endTime);
        Task<MasterSchedule> GetMasterScheduleByDateAndTimeRepo(DateOnly bookingDate, TimeOnly startTime, string masterId);
        Task<MasterSchedule> GetMasterScheduleByMasterIdAndStartTimeEndTimeAndDate(string masterId, TimeOnly startTime, TimeOnly endTime, DateOnly date);
        Task<List<MasterSchedule>> GetMasterScheduleByMasterIdAndDate(string masterId, DateOnly dateOnly);
    }
}
