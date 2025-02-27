using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class KoiVarietyDAO
    {
        private readonly KoiFishPondContext _context;

        public KoiVarietyDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<KoiVariety> GetKoiVarietyById(string koiVarietyId)
        {
            return await _context.KoiVarieties.FindAsync(koiVarietyId);
        }

        public async Task<List<KoiVariety>> GetKoiVarieties()
        {
            return _context.KoiVarieties.ToList();
        }

        public async Task<KoiVariety> CreateKoiVariety(KoiVariety koiVariety)
        {
            _context.KoiVarieties.Add(koiVariety);
            await _context.SaveChangesAsync();
            return koiVariety;
        }

        public async Task<KoiVariety> UpdateKoiVariety(KoiVariety koiVariety)
        {
            _context.KoiVarieties.Update(koiVariety);
            await _context.SaveChangesAsync();
            return koiVariety;
        }

        public async Task DeleteKoiVariety(string koiVarietyId)
        {
            var koiVariety = await GetKoiVarietyById(koiVarietyId);
            _context.KoiVarieties.Remove(koiVariety);
            await _context.SaveChangesAsync();
        }

    }
}
