using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.WorkShopRepository
{
    public class WorkShopRepo : IWorkShopRepo
    {
        public Task<WorkShop> GetWorkShopById(string workShopId)
        {
            return WorkShopDAO.Instance.GetWorkShopByIdDao(workShopId);
        }
        public Task<List<WorkShop>> GetWorkShops()
        {
            return WorkShopDAO.Instance.GetWorkShopsDao();
        }
        public Task<string> GetMasterIdByAccountId(string accountId)
        {
            return WorkShopDAO.Instance.GetMasterIdByAccountIdDao(accountId);
        }
        public Task<List<WorkShop>> SortingWorkshopByCreatedDate()
        {
            return WorkShopDAO.Instance.SortingWorkshopByCreatedDateDao();
        }
        public Task<WorkShop> CreateWorkShop(WorkShop workShop)
        {
            return WorkShopDAO.Instance.CreateWorkShopDao(workShop);
        }
        public Task<WorkShop> UpdateWorkShop(WorkShop workShop)
        {
            return WorkShopDAO.Instance.UpdateWorkShopDao(workShop);
        }
        public Task DeleteWorkShop(string workShopId)
        {
            return WorkShopDAO.Instance.DeleteWorkShopDao(workShopId);
        }
    }
}
