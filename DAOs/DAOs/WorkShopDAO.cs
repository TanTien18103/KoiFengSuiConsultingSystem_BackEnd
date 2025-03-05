using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class WorkShopDAO
    {
        public static WorkShopDAO instance = null;
        private readonly KoiFishPondContext _context;

        public WorkShopDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static WorkShopDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WorkShopDAO();
                }
                return instance;
            }
        }

        public async Task<WorkShop> GetWorkShopByIdDao(string workShopId)
        {
            return await _context.WorkShops.FindAsync(workShopId);
        }

        public async Task<List<WorkShop>> GetWorkShopsDao()
        {
            return _context.WorkShops.ToList();
        }

        public async Task<WorkShop> CreateWorkShopDao(WorkShop workShop)
        {
            _context.WorkShops.Add(workShop);
            await _context.SaveChangesAsync();
            return workShop;
        }

        public async Task<WorkShop> UpdateWorkShopDao(WorkShop workShop)
        {
            _context.WorkShops.Update(workShop);
            await _context.SaveChangesAsync();
            return workShop;
        }

        public async Task DeleteWorkShopDao(string workShopId)
        {
            var workShop = await GetWorkShopByIdDao(workShopId);
            _context.WorkShops.Remove(workShop);
            await _context.SaveChangesAsync();
        }
    }
}
