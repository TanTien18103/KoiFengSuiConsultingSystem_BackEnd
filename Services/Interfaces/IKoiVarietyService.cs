using BusinessObjects.Models;
using DAOs.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IKoiVarietyService
    {
        Task<KoiVariety> GetKoiVarietyByIdAsync(string koiVarietyId);
        Task<List<KoiVariety>> GetKoiVarietiesAsync();
        Task<KoiVariety> CreateKoiVarietyAsync(KoiVariety koiVariety);
        Task<KoiVariety> UpdateKoiVarietyAsync(KoiVariety koiVariety);
        Task DeleteKoiVarietyAsync(string koiVarietyId);
        Task<List<FishesWithColorsDTO>> GetKoiVarietyWithColorsAsync();
        Task<FishesWithColorsDTO> GetKoiVarietyWithColorsByIdAsync(string id);
    }
}
