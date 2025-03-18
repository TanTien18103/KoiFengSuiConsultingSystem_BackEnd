using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class WorkShopDAO
    {
        private static volatile WorkShopDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private WorkShopDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static WorkShopDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new WorkShopDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<WorkShop> GetWorkShopByIdDao(string workShopId)
        {
            return await _context.WorkShops.Include(x => x.Master).ThenInclude(x => x.Account)
                .FirstOrDefaultAsync(x => x.WorkshopId == workShopId);
        }

        public async Task<List<WorkShop>> GetWorkShopsDao()
        {
            return await _context.WorkShops.Include(x => x.Master).ToListAsync();
        }

        public async Task<string> GetMasterIdByAccountIdDao(string accountId)
        {
            var master = await _context.Masters.FirstOrDefaultAsync(x => x.AccountId == accountId);
            return master?.MasterId; 
        }

        public async Task<WorkShop> GetWorkshopByMasterLocationAndDateDao(string masterId, string location, DateTime? startDate)
        {
            return await _context.WorkShops.FirstOrDefaultAsync(w => w.MasterId == masterId && w.Location == location && w.StartDate == startDate);
        }

        public async Task<WorkShop> GetWorkshopByLocationAndDate(string location, DateTime? startDate)
        {
            return await _context.WorkShops.FirstOrDefaultAsync(w => w.Location == location && w.StartDate == startDate);
        }

        public async Task<List<WorkShop>> GetWorkshopsByMasterDao(string masterId)
        {
            return await _context.WorkShops.Where(w => w.MasterId == masterId).ToListAsync();
        }

        public async Task<List<WorkShop>> SortingWorkshopByCreatedDateDao()
        {
            return await _context.WorkShops
                .Include(x => x.Master).ThenInclude(x => x.Account)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
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
