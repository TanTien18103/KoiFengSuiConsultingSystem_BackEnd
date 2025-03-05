using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class KoiPondDAO
    {
        public static KoiPondDAO instance = null;
        private readonly KoiFishPondContext _context;

        public KoiPondDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static KoiPondDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KoiPondDAO();
                }
                return instance;
            }
        }

        public async Task<KoiPond> GetKoiPondByIdDao(string koiPondId)
        {
            return await _context.KoiPonds.FindAsync(koiPondId);
        }

        public async Task<List<KoiPond>> GetKoiPondsDao()  
        {
            return await _context.KoiPonds.Include(x => x.Shape).ToListAsync();
        }

        public async Task<KoiPond> CreateKoiPondDao(KoiPond koiPond)
        {
            _context.KoiPonds.Add(koiPond);
            await _context.SaveChangesAsync();
            return koiPond;
        }

        public async Task<KoiPond> UpdateKoiPondDao(KoiPond koiPond)
        {
            _context.KoiPonds.Update(koiPond);
            await _context.SaveChangesAsync();
            return koiPond;
        }

        public async Task DeleteKoiPondDao(string koiPondId)
        {
            var koiPond = await GetKoiPondByIdDao(koiPondId);
            _context.KoiPonds.Remove(koiPond);
            await _context.SaveChangesAsync();
        }
    }
}
