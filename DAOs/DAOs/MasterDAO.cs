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
        public static MasterDAO instance = null;
        private readonly KoiFishPondContext _context;

        public MasterDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static MasterDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MasterDAO();
                }
                return instance;
            }
        }

        public async Task<Master> GetByMasterIdDao(string masterId)
        {
            return await _context.Masters.FindAsync(masterId);
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
