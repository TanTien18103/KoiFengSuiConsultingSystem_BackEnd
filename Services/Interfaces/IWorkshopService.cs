using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IWorkshopService
    {
        Task<ResultModel> SortingWorkshopByCreatedDate();
        Task<ResultModel> ApprovedWorkshop(string id);
        Task<ResultModel> RejectedWorkshop(string id);
        Task<ResultModel> TrendingWorkshop(bool? trending = null);
    }
}
