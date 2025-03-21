using AutoMapper;
using BusinessObjects.Constants;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.ConsultationPackageRepository;
using Services.ApiModels;
using Services.ApiModels.ConsultationPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.ConsultationPackageService
{
    public class ConsultationPackageService : IConsultationPackageService
    {
        private readonly IConsultationPackageRepo _consultationPackageRepo;
        private readonly IMapper _mapper;
        public ConsultationPackageService(IConsultationPackageRepo consultationPackageRepo, IMapper mapper)
        {
            _consultationPackageRepo = consultationPackageRepo;
            _mapper = mapper;
        }

        public async Task<ResultModel> GetConsultationPackageById(string id)
        {
            var res = new ResultModel();
            try
            {
                var package = await _consultationPackageRepo.GetConsultationPackageById(id);
                if(package == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsPackage.PACKAGE_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsPackage.PACKAGE_FOUND;
                res.Data = _mapper.Map<ConsultationPackageResponse>(package);
                return res;
            }
            catch(Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetConsultationPackages()
        {
            var res = new ResultModel();
            try
            {
                var packages = await _consultationPackageRepo.GetConsultationPackages();
                if (packages == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsPackage.PACKAGE_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsPackage.PACKAGE_FOUND;
                res.Data = _mapper.Map<List<ConsultationPackageResponse>>(packages);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }
    }
}
