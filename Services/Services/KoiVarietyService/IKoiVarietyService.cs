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
        Task<ResultModel> GetKoiVarietyByIdAsync(string koiVarietyId);
        Task<ResultModel> GetKoiVarietiesAsync();
        Task<ResultModel> GetKoiVarietyWithColorsAsync();
        Task<ResultModel> GetKoiVarietyWithColorsByIdAsync(string id);
        Task<ResultModel> GetKoiVarietiesByElementAsync(NguHanh element);
        Task<ResultModel> GetRecommendedKoiVarietiesAsync();
        Task<ResultModel> GetKoiVarietiesByName(string name);
        Task<ResultModel> GetKoiVarietiesByColorsAsync(List<ColorEnums> colors);
        (bool IsCompatible, List<NguHanh> Elements, string Message) GetCompatibleElementsForColors(List<ColorEnums> colors);
        List<ColorEnums> GetPositiveColorsByElement(NguHanh nguHanh);
        Task<ResultModel> FilterByColorAndElement(NguHanh? nguHanh = null, List<ColorEnums>? colors = null);

        Task<ResultModel> CreateKoiVarietyAsync(KoiVarietyRequest koiVariety);
        Task<ResultModel> UpdateKoiVarietyAsync(string id, KoiVarietyRequest koiVariety);
        Task<ResultModel> DeleteKoiVarietyAsync(string id);


        Task<ResultModel> GetColorById(string id);
        Task<ResultModel> GetColors();
        Task<ResultModel> CreateColors(ColorRequest color);
        Task<ResultModel> UpdateColors(string id, ColorRequest color);
        Task<ResultModel> DeleteColors(string id);
    }
}
