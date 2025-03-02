using BusinessObjects.Models;
using DAOs.DTOs;
using Microsoft.EntityFrameworkCore;
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


        public async Task<List<FishesWithColorsDTO>> GetAllKoiVarietiesWithColors()
        {
            var koiVarieties = await _context.KoiVarieties
        .Include(k => k.VarietyColors)
            .ThenInclude(vc => vc.Color)
        .ToListAsync();

            return koiVarieties.Select(k => new FishesWithColorsDTO
            {
                Id = k.KoiVarietyId,
                VarietyName = k.VarietyName,
                Colors = k.VarietyColors
                    .Where(vc => vc.Color != null) 
                    .Select(vc => new ColorPercentageDto
                    {
                        ColorName = vc.Color.ColorName,
                        ColorCode = vc.Color.ColorCode,
                        Percentage = vc.Percentage ?? 0 
                    }).ToList()
            }).ToList();
        }

        public async Task<FishesWithColorsDTO> GetAllKoiVarietiesWithColorsById(string koiVarietyId)
        {
            var koiVariety = await _context.KoiVarieties
                .Include(k => k.VarietyColors)
                    .ThenInclude(vc => vc.Color)
                .FirstOrDefaultAsync(k => k.KoiVarietyId == koiVarietyId);

            if (koiVariety == null) return null;

            return new FishesWithColorsDTO
            {
                Id = koiVariety.KoiVarietyId,
                VarietyName = koiVariety.VarietyName,
                Colors = koiVariety.VarietyColors
                    .Where(vc => vc.Color != null)
                    .Select(vc => new ColorPercentageDto
                    {
                        ColorName = vc.Color.ColorName,
                        ColorCode = vc.Color.ColorCode,
                        Percentage = vc.Percentage ?? 0
                    }).ToList()
            };
        }

    }
}
