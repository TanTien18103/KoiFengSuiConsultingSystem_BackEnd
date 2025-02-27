using BusinessObjects.Models;
using DAOs.DAOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class WorkShopRepo : IWorkShopRepo
    {
        private readonly WorkShopDAO _workShopDAO;

        public WorkShopRepo(WorkShopDAO workShopDAO)
        {
            _workShopDAO = workShopDAO;
        }

        public async Task<WorkShop> GetWorkShopById(string workShopId)
        {
            return await _workShopDAO.GetWorkShopById(workShopId);
        }

        public async Task<WorkShop> CreateWorkShop(WorkShop workShop)
        {
            return await _workShopDAO.CreateWorkShop(workShop);
        }

        public async Task<WorkShop> UpdateWorkShop(WorkShop workShop)
        {
            return await _workShopDAO.UpdateWorkShop(workShop);
        }

        public async Task DeleteWorkShop(string workShopId)
        {
            await _workShopDAO.DeleteWorkShop(workShopId);
        }

        public async Task<List<WorkShop>> GetWorkShops()
        {
            return await _workShopDAO.GetWorkShops();
        }
    }
}
