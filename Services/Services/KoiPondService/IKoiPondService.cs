using BusinessObjects.Models;
using Services.ApiModels;
using Services.ApiModels.KoiPond;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.KoiPondService
{
    public interface IKoiPondService
    {
        Task<ResultModel> GetAllKoiPonds();
        Task<ResultModel> GetKoiPondById(string id);
        Task<ResultModel> GetPondRecommendations();
    }
}