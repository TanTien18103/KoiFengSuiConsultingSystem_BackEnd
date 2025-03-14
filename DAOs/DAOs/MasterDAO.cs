using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class MasterDAO
    {
        private static volatile MasterDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private MasterDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static MasterDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new MasterDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Master> GetByMasterIdDao(string masterId)
        {
            return await _context.Masters.FindAsync(masterId);
        }

        public async Task<Master> GetMasterByAccountIdDao(string accountId)
        {
            return await _context.Masters
                .Include(x => x.MasterSchedules)
                .Include(x => x.Account)
                .Where(x => x.AccountId == accountId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Master>> GetAllMastersDAO() 
        {
            return await _context.Masters.ToListAsync();
        }

        public async Task<T> CreateDao<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateDao<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
