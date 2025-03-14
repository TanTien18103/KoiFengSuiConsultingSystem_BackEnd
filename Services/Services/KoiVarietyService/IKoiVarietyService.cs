using BusinessObjects.Enums;
using BusinessObjects.Models;
using Services.ApiModels;
using Services.ApiModels.KoiVariety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.KoiVarietyService
{
    public interface IKoiVarietyService
    {
        Task<KoiVariety> GetKoiVarietyByIdAsync(string koiVarietyId);
        Task<List<KoiVariety>> GetKoiVarietiesAsync();
        Task<KoiVariety> CreateKoiVarietyAsync(KoiVariety koiVariety);
        Task<KoiVariety> UpdateKoiVarietyAsync(KoiVariety koiVariety);
        Task DeleteKoiVarietyAsync(string koiVarietyId);
        Task<ResultModel> GetKoiVarietyWithColorsAsync();
        Task<ResultModel> GetKoiVarietyWithColorsByIdAsync(string id);
        Task<ResultModel> GetKoiVarietiesByElementAsync(string element);
        Task<ResultModel> GetRecommendedKoiVarietiesAsync();
    }
}
