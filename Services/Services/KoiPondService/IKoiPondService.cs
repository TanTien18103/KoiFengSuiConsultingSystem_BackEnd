using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Services.ApiModels;
using Services.ApiModels.KoiPond;
using Services.ApiModels.KoiVariety;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.KoiPondService
{
    public interface IKoiPondService
    {
        Task<ResultModel> GetAllKoiPonds();
        Task<ResultModel> GetKoiPondById(string id);
        Task<ResultModel> GetPondRecommendations();
        Task<ResultModel> CreateKoiPond(KoiPondRequest koiPond);
        Task<ResultModel> UpdateKoiPond(string id, KoiPondRequest koiPond);
        Task<ResultModel> DeleteKoiPond(string id);
        Task<ResultModel> GetKoiPondByShapeId(string shapeId);
        Task<ResultModel> GetAllShapes();
        Task<ResultModel> GetKoiPondsByName(string name);
    }
}