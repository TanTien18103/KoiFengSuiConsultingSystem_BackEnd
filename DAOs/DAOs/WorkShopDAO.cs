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
        private readonly KoiFishPondContext _context;

        public WorkShopDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<WorkShop> GetWorkShopById(string workShopId)
        {
            return await _context.WorkShops.FindAsync(workShopId);
        }

        public async Task<List<WorkShop>> GetWorkShops()
        {
            return _context.WorkShops.ToList();
        }

        public async Task<WorkShop> CreateWorkShop(WorkShop workShop)
        {
            _context.WorkShops.Add(workShop);
            await _context.SaveChangesAsync();
            return workShop;
        }

        public async Task<WorkShop> UpdateWorkShop(WorkShop workShop)
        {
            _context.WorkShops.Update(workShop);
            await _context.SaveChangesAsync();
            return workShop;
        }

        public async Task DeleteWorkShop(string workShopId)
        {
            var workShop = await GetWorkShopById(workShopId);
            _context.WorkShops.Remove(workShop);
            await _context.SaveChangesAsync();
        }
    }
}
