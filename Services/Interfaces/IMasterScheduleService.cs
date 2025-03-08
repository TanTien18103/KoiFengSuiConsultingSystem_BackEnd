using Services.ApiModels.Master;
using Services.ApiModels;
using Services.ApiModels.MasterSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface IMasterScheduleService
    {
        Task<ResultModel> GetAllMasterSchedules();
        Task<ResultModel> GetMasterSchedulesByMasterId(string masterId);
        Task<ResultModel> GetMasterSchedulesByMasterAndDate(string masterId, DateTime date);
        Task<MasterSchedule> CreateMasterSchedule(MasterSchedule schedule);
    }
}
