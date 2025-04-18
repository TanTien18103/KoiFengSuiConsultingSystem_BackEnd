using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.MasterScheduleRepository
{
    public class MasterScheduleRepo : IMasterScheduleRepo
    {
        public Task<MasterSchedule> GetMasterScheduleById(string masterScheduleId)
        {
            return MasterScheduleDAO.Instance.GetMasterScheduleByIdDao(masterScheduleId);
        }
        public Task<MasterSchedule> GetMasterSchedule(string masterId, DateOnly date, TimeOnly startTime)
        {
            return MasterScheduleDAO.Instance.GetMasterScheduleDao(masterId, date, startTime);
        }
        public Task<List<MasterSchedule>> GetAllSchedules()
        {
            return MasterScheduleDAO.Instance.GetAllSchedulesAsync();
        }
        public Task<List<MasterSchedule>> GetSchedulesByMasterId(string masterId)
        {
            return MasterScheduleDAO.Instance.GetSchedulesByMasterIdAsync(masterId);
        }
        public Task<List<MasterSchedule>> GetSchedulesByMasterAndDate(string masterId, DateOnly date)
        {
            return MasterScheduleDAO.Instance.GetSchedulesByMasterAndDateAsync(masterId, date);
        }
        public Task<MasterSchedule> CreateMasterSchedule(MasterSchedule masterSchedule)
        {
            return MasterScheduleDAO.Instance.CreateMasterScheduleDao(masterSchedule);
        }
        public Task<MasterSchedule> UpdateMasterSchedule(MasterSchedule masterSchedule)
        {
            return MasterScheduleDAO.Instance.UpdateMasterScheduleDao(masterSchedule);
        }
        public Task<MasterSchedule> GetMasterScheduleByMasterIdAndWorkshopId(string masterId, string workshopId)
        {
            return MasterScheduleDAO.Instance.GetMasterScheduleByMasterIdAndWorkshopIdDao(masterId, workshopId);
        }
        public Task DeleteMasterSchedule(string masterScheduleId)
        {
            return MasterScheduleDAO.Instance.DeleteMasterScheduleDao(masterScheduleId);
        }
        public Task<List<MasterSchedule>> GetMasterScheduleByMasterId(string masterId)
        {
            return MasterScheduleDAO.Instance.GetMasterScheduleByMasterId(masterId);
        }
        public Task<bool> CheckMasterScheduleAvailabilityRepo(string masterId, DateOnly? bookingDate, TimeOnly? startTime, TimeOnly? endTime)
        {
            return MasterScheduleDAO.Instance.CheckMasterScheduleAvailabilityDao(masterId, bookingDate, startTime, endTime);
        }
        public Task<MasterSchedule> GetMasterScheduleByDateAndTimeRepo(DateOnly bookingDate, TimeOnly startTime, string masterId)
        {
            return MasterScheduleDAO.Instance.GetMasterScheduleByDateAndTimeDao(bookingDate, startTime, masterId);
        }

        public Task<MasterSchedule> GetMasterScheduleByMasterIdAndStartTimeEndTimeAndDate(string masterId, TimeOnly startTime, TimeOnly endTime, DateOnly date)
        {
            return MasterScheduleDAO.Instance.GetMasterScheduleByMasterIdAndStartTimeEndTimeAndDate(masterId, startTime, endTime, date);
        }

        public Task<List<MasterSchedule>> GetMasterScheduleByMasterIdAndDate(string masterId, DateOnly dateOnly)
        {
            return MasterScheduleDAO.Instance.GetMasterScheduleByMasterIdAndDateDao(masterId, dateOnly);
        }
    }
}
