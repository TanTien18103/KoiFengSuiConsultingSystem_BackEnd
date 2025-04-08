using Services.ApiModels;
using Services.ApiModels.Workshop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.WorkshopService
{
    public interface IWorkshopService
    {
        Task<ResultModel> SortingWorkshopByCreatedDate();
        Task<ResultModel> ApprovedWorkshop(string id);
        Task<ResultModel> RejectedWorkshop(string id);
        Task<ResultModel> TrendingWorkshop();
        Task<ResultModel> GetWorkshopById(string id);
        Task<ResultModel> CreateWorkshop(WorkshopRequest workshopRequest);
        Task<ResultModel> UpdateWorkshop(string id, WorkshopUpdateRequest workshopRequest);
        Task<ResultModel> DeleteWorkshop(string id);
        Task<ResultModel> CheckIn(string workshopId, string registerId);

    }
}
