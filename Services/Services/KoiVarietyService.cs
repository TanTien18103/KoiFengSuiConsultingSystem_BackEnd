using BusinessObjects.Models;
using DAOs.DTOs;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class KoiVarietyService : IKoiVarietyService
    {
        private readonly IKoiVarietyRepo _koiVarietyRepo;

        public KoiVarietyService(IKoiVarietyRepo koiVarietyRepo)
        {
            _koiVarietyRepo = koiVarietyRepo;
        }

        public async Task<KoiVariety> CreateKoiVarietyAsync(KoiVariety koiVariety)
        {
            return await _koiVarietyRepo.CreateKoiVariety(koiVariety);
        }

        public async Task DeleteKoiVarietyAsync(string koiVarietyId)
        {
            await _koiVarietyRepo.DeleteKoiVariety(koiVarietyId);
        }

        public async Task<List<KoiVariety>> GetKoiVarietiesAsync()
        {
            return await _koiVarietyRepo.GetKoiVarieties();
        }

        public async Task<KoiVariety> GetKoiVarietyByIdAsync(string koiVarietyId)
        {
            return await _koiVarietyRepo.GetKoiVarietyById(koiVarietyId);
        }

       
        public async Task<KoiVariety> UpdateKoiVarietyAsync(KoiVariety koiVariety)
        {
            return await _koiVarietyRepo.UpdateKoiVariety(koiVariety);
        }

        public async Task<List<FishesWithColorsDTO>> GetKoiVarietyWithColorsAsync()
        {
            return await _koiVarietyRepo.GetKoiVarietyWithColors();
        }

        public async Task<FishesWithColorsDTO> GetKoiVarietyWithColorsByIdAsync(string id)
        {
            return await _koiVarietyRepo.GetKoiVarietyWithColorsById(id);
        }
    }
}
