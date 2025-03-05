using BusinessObjects.Models;
using DAOs.DTOs;
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
        public static KoiVarietyDAO instance = null;
        private readonly KoiFishPondContext _context;

        public KoiVarietyDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static KoiVarietyDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KoiVarietyDAO();
                }
                return instance;
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

        public async Task<List<KoiVarietyElementDTO>> GetKoiVarietiesByCustomerElementDao(string element)
        {
            var query = from vc in _context.VarietyColors
                        join kv in _context.KoiVarieties on vc.KoiVarietyId equals kv.KoiVarietyId
                        join c in _context.Colors on vc.ColorId equals c.ColorId
                        where c.Element == element
                        group new { kv, c } by new { kv.VarietyName, c.Element } into g
                        select new KoiVarietyElementDTO
                        {
                            VarietyName = g.Key.VarietyName,
                            Element = g.Key.Element
                        };

            return await query.ToListAsync();
        }

    }
}
