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
        private readonly KoiFishPondContext _context;

        public MasterDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<T> GetById<T>(string id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetAll<T>() where T : class
        {
            return _context.Set<T>().ToList();
        }

        public async Task<T> Create<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Update<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete<T>(string id) where T : class
        {
            var entity = await GetById<T>(id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
