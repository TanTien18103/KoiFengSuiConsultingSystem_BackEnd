using Services.ApiModels;
using Services.ApiModels.ConsultationPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.ConsultationPackageService
{
    public interface IConsultationPackageService
    {
        Task<ResultModel> GetConsultationPackageById(string id);
        Task<ResultModel> GetConsultationPackages();
        Task<ResultModel> CreateConsultationPackage(ConsultationPackageRequest consultationPackageRequest);
        Task<ResultModel> UpdateConsultationPackage(string id, ConsultationPackageRequest consultationPackageRequest);
        Task<ResultModel> DeleteConsultationPackage(string id);
    }
}
