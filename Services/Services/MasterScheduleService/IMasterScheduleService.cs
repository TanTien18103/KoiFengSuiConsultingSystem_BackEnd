using Services.ApiModels.Master;
using Services.ApiModels;
using Services.ApiModels.MasterSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Services.Services.MasterScheduleService
{
    public interface IMasterScheduleService
    {
        Task<ResultModel> GetAllMasterSchedules();
        Task<ResultModel> GetMasterSchedulesByCurrentMasterLogin();
        Task<ResultModel> GetMasterSchedulesByMasterAndDate(string masterId, DateTime date);
        Task<MasterSchedule> CreateMasterSchedule(MasterSchedule schedule);
    }
}
