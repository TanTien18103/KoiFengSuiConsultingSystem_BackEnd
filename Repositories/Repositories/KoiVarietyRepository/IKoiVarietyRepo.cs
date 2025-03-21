using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.KoiVarietyRepository
{
    public interface IKoiVarietyRepo
    {
        Task<KoiVariety> GetKoiVarietyById(string id);
        Task<List<KoiVariety>> GetKoiVarieties();
        Task<List<KoiVariety>> GetKoiVarietyWithColors();
        Task<KoiVariety> GetKoiVarietyWithColorsById(string id);
        Task<List<KoiVariety>> GetKoiVarietiesByElement(string element);
        Task<KoiVariety> CreateKoiVariety(KoiVariety koiVariety);
        Task<KoiVariety> UpdateKoiVariety(KoiVariety koiVariety);
        Task DeleteKoiVariety(string id);
        Task<List<KoiVariety>> GetKoiVarietiesByName(string name);
        Task<List<KoiVariety>> GetKoiVarietiesByColors(List<string> colorIds);
    }
}
