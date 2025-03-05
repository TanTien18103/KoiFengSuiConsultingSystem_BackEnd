using BusinessObjects.Models;
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

        public async Task<T> GetByIdDao<T>(string id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetAllDao<T>() where T : class
        {
            return _context.Set<T>().ToList();
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

        public async Task DeleteDao<T>(string id) where T : class
        {
            var entity = await GetByIdDao<T>(id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
