using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.ConsultationPackageRepository;
using Services.ApiModels;
using Services.ApiModels.Category;
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
                if (consultationPackageRequest == null)
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
                package.Status = ConsultationPackageStatusEnums.Inactive.ToString();
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
                if (package == null)
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
            catch (Exception ex)
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

        public async Task<ResultModel> UpdateConsultationPackage(string id, ConsultationPackageUpdateRequest consultationPackageRequest)
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

                // Gán giá trị tạm thời để check logic Min < Max
                var tempMin = consultationPackageRequest.MinPrice ?? package.MinPrice;
                var tempMax = consultationPackageRequest.MaxPrice ?? package.MaxPrice;

                if (tempMin > tempMax)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsPackage.INVALID_PRICE_RANGE;
                    return res;
                }


                // Cập nhật từng trường nếu có giá trị
                if (!string.IsNullOrWhiteSpace(consultationPackageRequest.PackageName))
                    package.PackageName = consultationPackageRequest.PackageName;

                if (consultationPackageRequest.MinPrice.HasValue)
                    package.MinPrice = consultationPackageRequest.MinPrice.Value;

                if (consultationPackageRequest.MaxPrice.HasValue)
                    package.MaxPrice = consultationPackageRequest.MaxPrice.Value;

                if (!string.IsNullOrWhiteSpace(consultationPackageRequest.Description))
                    package.Description = consultationPackageRequest.Description;

                if (!string.IsNullOrWhiteSpace(consultationPackageRequest.SuitableFor))
                    package.SuitableFor = consultationPackageRequest.SuitableFor;

                if (!string.IsNullOrWhiteSpace(consultationPackageRequest.RequiredInfo))
                    package.RequiredInfo = consultationPackageRequest.RequiredInfo;

                if (!string.IsNullOrWhiteSpace(consultationPackageRequest.PricingDetails))
                    package.PricingDetails = consultationPackageRequest.PricingDetails;

                // Nếu có ảnh mới thì upload
                if (consultationPackageRequest.ImageUrl != null)
                {
                    package.ImageUrl = await _uploadService.UploadImageAsync(consultationPackageRequest.ImageUrl);
                }

                await _consultationPackageRepo.UpdateConsultationPackage(package);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsPackage.PACKAGE_UPDATED;
                res.Data = _mapper.Map<ConsultationPackageResponse>(package);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Lỗi khi cập nhật gói tư vấn: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> UpdateConsultationPackageStatus(string id, ConsultationPackageStatusEnums status)
        {
            var res = new ResultModel();

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsPackage.CONSULTATION_PACKAGE_ID_INVALID;
                    return res;
                }

                // Làm sạch id
                id = id.Trim();

                // Validate status enum (phòng trường hợp nhận từ body dạng int bị sai)
                if (!Enum.IsDefined(typeof(ConsultationPackageStatusEnums), status))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsPackage.STATUS_INVALID;
                    return res;
                }

                var consultationPackage = await _consultationPackageRepo.GetConsultationPackageById(id);
                if (consultationPackage == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsPackage.CONSULTATION_PACKAGE_NOT_FOUND;
                    return res;
                }

                var newStatus = status.ToString();

                if (consultationPackage.Status == newStatus)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsPackage.CONSULTATION_PACKAGE_ALREADY_HAS_THIS_STATUS;
                    return res;
                }

                consultationPackage.Status = newStatus;
                await _consultationPackageRepo.UpdateConsultationPackage(consultationPackage);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<ConsultationPackageResponse>(consultationPackage);
                res.Message = ResponseMessageConstrantsPackage.PACKAGE_STATUS_UPDATED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = "Đã xảy ra lỗi nội bộ: " + ex.Message;
                return res;
            }
        }
    }
}
