using BusinessObjects.Models;
using KoiFengSuiConsultingSystem.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IKoiPondService
    {
        Task<List<KoiPondResponse>> GetAllKoiPonds();
        Task<KoiPondResponse> GetKoiPondById(string id);
        Task<object> GetPondRecommendations();
    }
}