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
            return await _context.WorkShops
                .Include(x => x.Master).ThenInclude(x => x.Account)
                .Include(x => x.Location)
                .FirstOrDefaultAsync(x => x.WorkshopId == workShopId);
        }

        public async Task<List<WorkShop>> GetWorkShopsDao()
        {
            return await _context.WorkShops
                .Include(x => x.Master)
                .Include(x => x.Location)
                .ToListAsync();
        }

        // Hàm tìm theo MasterId, LocationId và StartDate
        public async Task<WorkShop> GetWorkshopByMasterLocationAndDateDao(string masterId, string locationId, DateTime? startDate)
        {
            return await _context.WorkShops
                .FirstOrDefaultAsync(w => w.MasterId == masterId && w.LocationId == locationId && w.StartDate == startDate);
        }

        // Hàm tìm theo LocationId và StartDate
        public async Task<WorkShop> GetWorkshopByLocationAndDate(string locationId, DateTime? startDate)
        {
            return await _context.WorkShops
                .FirstOrDefaultAsync(w => w.LocationId == locationId && w.StartDate == startDate);
        }


        public async Task<List<WorkShop>> GetWorkshopsByMasterDao(string masterId)
        {
            return await _context.WorkShops.Where(w => w.MasterId == masterId).ToListAsync();
        }

        public async Task<List<WorkShop>> SortingWorkshopByCreatedDateDao()
        {
            return await _context.WorkShops
                .Include(x => x.Master).ThenInclude(x => x.Account)
                .Include(x => x.Location)
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

        public async Task<List<WorkShop>> GetWorkshopsByDateDao(DateTime value)
        {
            return await _context.WorkShops
                .Where(x => x.StartDate == value)
                .Include(x => x.Master).ThenInclude(x => x.Account)
                .Include(x => x.Location)
                .ToListAsync();
        }
    }
}
