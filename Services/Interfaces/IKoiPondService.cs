using BusinessObjects.Models;
using Services.ApiModels;
using Services.ApiModels.KoiPond;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IKoiPondService
    {
        Task<ResultModel<List<KoiPondResponse>>> GetAllKoiPonds();
        Task<ResultModel<KoiPondResponse>> GetKoiPondById(string id);
        Task<ResultModel> GetPondRecommendations();
    }
}