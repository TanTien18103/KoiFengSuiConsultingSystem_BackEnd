using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.WorkShopRepository
{
    public interface IWorkShopRepo
    {
        Task<WorkShop> GetWorkShopById(string workShopId);
        Task<string> GetMasterIdByAccountId(string accountId);
        Task<List<WorkShop>> GetWorkShops();
        Task<List<WorkShop>> SortingWorkshopByCreatedDate();
        Task<WorkShop> CreateWorkShop(WorkShop workShop);
        Task<WorkShop> UpdateWorkShop(WorkShop workShop);
        Task DeleteWorkShop(string workShopId);
    }
}
