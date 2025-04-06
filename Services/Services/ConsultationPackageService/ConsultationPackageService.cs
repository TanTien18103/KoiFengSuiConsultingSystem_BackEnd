using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.ConsultationPackageRepository;
using Services.ApiModels;
using Services.ApiModels.ConsultationPackage;
using Services.ServicesHelpers.UploadService;
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
        private readonly IUploadService _uploadService;
        public ConsultationPackageService(IConsultationPackageRepo consultationPackageRepo, IMapper mapper, IUploadService uploadService)
        {
            _consultationPackageRepo = consultationPackageRepo;
            _mapper = mapper;
            _uploadService = uploadService;
        }

        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }

        public async Task<ResultModel> CreateConsultationPackage(ConsultationPackageRequest consultationPackageRequest)
        {
            var res = new ResultModel();
            try
            {
                if(consultationPackageRequest == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsPackage.PACKAGE_INVALID;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var package = _mapper.Map<ConsultationPackage>(consultationPackageRequest);
                package.ConsultationPackageId = GenerateShortGuid();
                package.ImageUrl = await _uploadService.UploadImageAsync(consultationPackageRequest.ImageUrl);
                var createdPackage = await _consultationPackageRepo.CreateConsultationPackage(package);

                if (createdPackage == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantsPackage.PACKAGE_CREATE_FAILED;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Message = ResponseMessageConstrantsPackage.PACKAGE_CREATED;
                res.Data = _mapper.Map<ConsultationPackageResponse>(createdPackage);
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

        public async Task<ResultModel> DeleteConsultationPackage(string id)
        {
            var res = new ResultModel();
            try
            {
                var package = await _consultationPackageRepo.GetConsultationPackageById(id);
                if (package == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsPackage.PACKAGE_NOT_FOUND;
                    return res;
                }

                await _consultationPackageRepo.DeleteConsultationPackage(id);
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsPackage.PACKAGE_DELETED;
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

        public async Task<ResultModel> UpdateConsultationPackage(string id, ConsultationPackageRequest consultationPackageRequest)
        {
            var res = new ResultModel();
            try
            {
                var package = await _consultationPackageRepo.GetConsultationPackageById(id);
                if (package == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsPackage.PACKAGE_NOT_FOUND;
                    return res;
                }

                var updatedPackage = _mapper.Map(consultationPackageRequest, package);
                updatedPackage.ImageUrl = await _uploadService.UploadImageAsync(consultationPackageRequest.ImageUrl);
                await _consultationPackageRepo.UpdateConsultationPackage(updatedPackage);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsPackage.PACKAGE_UPDATED;
                res.Data = _mapper.Map<ConsultationPackageResponse>(updatedPackage);
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
