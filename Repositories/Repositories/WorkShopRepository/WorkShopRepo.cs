﻿using BusinessObjects.Models;
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
        public Task<WorkShop> GetWorkshopByMasterLocationAndDate(string masterId, string location, DateTime? startDate)
        {
            return WorkShopDAO.Instance.GetWorkshopByMasterLocationAndDateDao(masterId, location, startDate);
        }
        public Task<WorkShop> GetWorkshopByLocationAndDate(string location, DateTime? startDate)
        {
            return WorkShopDAO.Instance.GetWorkshopByLocationAndDate(location, startDate);
        }
        public Task<List<WorkShop>> GetWorkshopsByMaster(string accountId)
        {
            return WorkShopDAO.Instance.GetWorkshopsByMasterDao(accountId);
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
