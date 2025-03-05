using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IKoiVarietyRepo
    {
        Task<KoiVariety> GetKoiVarietyById(string koiVarietyId);
        Task<List<KoiVariety>> GetKoiVarieties();
        Task<KoiVariety> CreateKoiVariety(KoiVariety koiVariety);
        Task<KoiVariety> UpdateKoiVariety(KoiVariety koiVariety);
        Task DeleteKoiVariety(string koiVarietyId);
        Task<List<FishesWithColorsDTO>> GetKoiVarietyWithColors();
        Task<FishesWithColorsDTO> GetKoiVarietyWithColorsById(string id);
        Task<List<KoiVarietyElementDTO>> GetKoiVarietiesByElement(string element);
        Task<List<KoiVariety>> GetKoiVarietyWithColors();
        Task<KoiVariety> GetKoiVarietyWithColorsById(string id);
    }
}
