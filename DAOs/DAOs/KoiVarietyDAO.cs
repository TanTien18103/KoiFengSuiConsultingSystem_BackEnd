using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DAOs.DAOs
{
    public class KoiVarietyDAO
    {
        private static volatile KoiVarietyDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private KoiVarietyDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static KoiVarietyDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new KoiVarietyDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<KoiVariety> GetKoiVarietyByIdDao(string koiVarietyId)
        {
            return await _context.KoiVarieties
                .Include(k => k.VarietyColors)
                    .ThenInclude(vc => vc.Color)
                .FirstOrDefaultAsync(k => k.KoiVarietyId == koiVarietyId);
        }

        public async Task<List<KoiVariety>> GetKoiVarietiesDao()
        {
            return await _context.KoiVarieties
                .Include(k => k.VarietyColors)
                    .ThenInclude(vc => vc.Color)
                .ToListAsync();
        }

        public async Task<List<KoiVariety>> GetAllKoiVarietiesWithColorsDao()
        {
            return await _context.KoiVarieties
                .Include(k => k.VarietyColors)
                .ThenInclude(vc => vc.Color)
                .ToListAsync();
        }

        public async Task<KoiVariety> GetAllKoiVarietiesWithColorsByIdDao(string koiVarietyId)
        {
            return await _context.KoiVarieties
                .Include(k => k.VarietyColors)
                .ThenInclude(vc => vc.Color)
                .FirstOrDefaultAsync(k => k.KoiVarietyId == koiVarietyId);
        }

        public async Task<List<KoiVariety>> GetKoiVarietiesByElementDao(string element)
        {
            return await _context.KoiVarieties
                .Include(k => k.VarietyColors)
                    .ThenInclude(vc => vc.Color)
                .Where(k => k.VarietyColors.Any(vc => vc.Color.Element == element))
                .ToListAsync();
        }

        public async Task<KoiVariety> CreateKoiVarietyDao(KoiVariety koiVariety)
        {
            _context.KoiVarieties.Add(koiVariety);
            await _context.SaveChangesAsync();
            return koiVariety;
        }

        public async Task<KoiVariety> UpdateKoiVarietyDao(KoiVariety koiVariety)
        {
            _context.KoiVarieties.Update(koiVariety);
            await _context.SaveChangesAsync();
            return koiVariety;
        }

        public async Task DeleteKoiVarietyDao(string koiVarietyId)
        {
            var koiVariety = await GetKoiVarietyByIdDao(koiVarietyId);
            if (koiVariety != null)
            {
                _context.KoiVarieties.Remove(koiVariety);
                await _context.SaveChangesAsync();
            }
        }
    }
}
