using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class KoiPondDAO
    {
        private readonly KoiFishPondContext _context;

        public KoiPondDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<KoiPond> GetKoiPondById(string koiPondId)
        {
            return await _context.KoiPonds.FindAsync(koiPondId);
        }

        public async Task<List<KoiPond>> GetKoiPonds()
        {
            return _context.KoiPonds.ToList();
        }

        public async Task<KoiPond> CreateKoiPond(KoiPond koiPond)
        {
            _context.KoiPonds.Add(koiPond);
            await _context.SaveChangesAsync();
            return koiPond;
        }

        public async Task<KoiPond> UpdateKoiPond(KoiPond koiPond)
        {
            _context.KoiPonds.Update(koiPond);
            await _context.SaveChangesAsync();
            return koiPond;
        }

        public async Task DeleteKoiPond(string koiPondId)
        {
            var koiPond = await GetKoiPondById(koiPondId);
            _context.KoiPonds.Remove(koiPond);
            await _context.SaveChangesAsync();
        }
    }
}
