﻿using BusinessObjects.Enums;
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
        Task<ResultModel> GetConsultationPackagesForMobile();
        Task<ResultModel> CreateConsultationPackage(ConsultationPackageRequest consultationPackageRequest);
        Task<ResultModel> UpdateConsultationPackage(string id, ConsultationPackageUpdateRequest consultationPackageRequest);
        Task<ResultModel> DeleteConsultationPackage(string id);
        Task<ResultModel> UpdateConsultationPackageStatus(string id, ConsultationPackageStatusEnums status);

    }
}
