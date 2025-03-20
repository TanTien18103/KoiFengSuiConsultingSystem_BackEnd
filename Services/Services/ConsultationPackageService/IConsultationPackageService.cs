using Services.ApiModels;
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
    }
}
