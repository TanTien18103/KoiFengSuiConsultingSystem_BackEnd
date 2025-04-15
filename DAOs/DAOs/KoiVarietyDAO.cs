using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

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

        public async Task<List<KoiVariety>> GetKoiVarietiesByNameDao(string name)
        {
            var koiVarieties = await _context.KoiVarieties
                .Include(k => k.VarietyColors)
                .ThenInclude(vc => vc.Color)
                .Where(x => x.VarietyName.ToLower().Contains(name.ToLower()))
                .ToListAsync();
            return koiVarieties;
        }

        public async Task<List<KoiVariety>> GetKoiVarietiesByColorsDao(List<ColorEnums> colors)
        {
            var colorNames = colors.Select(c => c.ToString()).ToList();

            return await _context.KoiVarieties
                .Include(k => k.VarietyColors)
                    .ThenInclude(vc => vc.Color)
                .Where(k => k.VarietyColors
                    .Any(vc => colorNames.Contains(vc.Color.ColorName)))
                .ToListAsync();
        }

        public async Task<KoiVariety> CreateKoiVarietyDao(KoiVariety koiVariety)
        {
            await _context.KoiVarieties.AddAsync(koiVariety);
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
            var koiVariety = await _context.KoiVarieties
                .Include(kv => kv.VarietyColors)
                .FirstOrDefaultAsync(kv => kv.KoiVarietyId == koiVarietyId);

            if (koiVariety != null)
            {
                _context.VarietyColors.RemoveRange(koiVariety.VarietyColors);
                await _context.SaveChangesAsync();

                _context.KoiVarieties.Remove(koiVariety);
                await _context.SaveChangesAsync();
            }
        }

    }
}
