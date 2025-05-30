﻿using BusinessObjects.Models;
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
        Task<List<WorkShop>> GetWorkShops();
        Task<WorkShop> GetWorkshopByMasterLocationAndDate(string masterId, string location, DateTime? startDate);
        Task<WorkShop> GetWorkshopByLocationAndDate(string location, DateTime? startDate);
        Task<List<WorkShop>> GetWorkshopsByMaster(string masterId);
        Task<List<WorkShop>> SortingWorkshopByCreatedDate();
        Task<List<WorkShop>> SortingWorkshopByCreatedDateForWeb(string masterId);
        Task<WorkShop> CreateWorkShop(WorkShop workShop);
        Task<WorkShop> UpdateWorkShop(WorkShop workShop);
        Task DeleteWorkShop(string workShopId);
        Task<List<WorkShop>> GetWorkshopsByDate(DateTime value);
    }
}
