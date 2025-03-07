using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IMasterScheduleRepository 
    {
        Task<List<MasterSchedule>> GetAllSchedulesAsync();
        Task<List<MasterSchedule>> GetSchedulesByMasterAndDateAsync(string masterId, DateOnly date);
    }
} 