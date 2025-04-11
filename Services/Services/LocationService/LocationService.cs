using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.CategoryRepository;
using Repositories.Repositories.LocationRepository;
using Services.ApiModels;
using Services.ApiModels.Category;
using Services.ApiModels.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.LocationService
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepo _locationRepo;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepo locationRepo, IMapper mapper)
        {
            _locationRepo = locationRepo;
            _mapper = mapper;
        }

        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }

        public async Task<ResultModel> CreateLocation(LocationRequest locationRequest)
        {
            var res = new ResultModel();
            try
            {
                var locations = await _locationRepo.GetLocationsRepo();
                var existingLocationName = locations.FirstOrDefault(x => x.LocationName == locationRequest.LocationName);

                if (existingLocationName != null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = ResponseMessageConstrantsLocation.LOCATION_ALREADY_EXIST;
                    return res;
                }

                var location = _mapper.Map<Location>(locationRequest);
                location.LocationId = GenerateShortGuid();
                location.LocationName = locationRequest.LocationName;
                
                await _locationRepo.CreateLocationRepo(location);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status201Created;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Data = _mapper.Map<LocationResponse>(location);
                res.Message = ResponseMessageConstrantsLocation.LOCATION_CREATED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> DeleteLocation(string id)
        {
            var res = new ResultModel();
            try
            {
                var category = await _locationRepo.GetLocationByIdRepo(id);
                if (category == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsLocation.LOCATION_NOT_FOUND;
                    return res;
                }

                await _locationRepo.DeleteLocationRepo(id);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsLocation.LOCATION_DELETED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetAllLocations()
        {
            var res = new ResultModel();
            try
            {
                var locations = await _locationRepo.GetLocationsRepo();
                if (locations == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsLocation.LOCATION_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Data = _mapper.Map<List<LocationResponse>>(locations);
                res.Message = ResponseMessageConstrantsLocation.LOCATION_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetLocationById(string id)
        {
            var res = new ResultModel();
            try
            {
                var location = await _locationRepo.GetLocationByIdRepo(id);
                if (location == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsLocation.LOCATION_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Data = _mapper.Map<LocationResponse>(location);
                res.Message = ResponseMessageConstrantsLocation.LOCATION_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> UpdateLocation(string id, LocationUpdateRequest locationRequest)
        {
            var res = new ResultModel();

            try
            {
                var location = await _locationRepo.GetLocationByIdRepo(id);
                if (location == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsLocation.LOCATION_NOT_FOUND;
                    return res;
                }

                // Kiểm tra trùng tên nếu có truyền LocationName
                if (!string.IsNullOrWhiteSpace(locationRequest.LocationName))
                {
                    var locations = await _locationRepo.GetLocationsRepo();
                    var existingLocationName = locations.FirstOrDefault(x =>
                        x.LocationName == locationRequest.LocationName && x.LocationId != id);

                    if (existingLocationName != null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status400BadRequest;
                        res.ResponseCode = ResponseCodeConstants.FAILED;
                        res.Message = ResponseMessageConstrantsCategory.CATEGORY_ALREADY_EXIST;
                        return res;
                    }

                    location.LocationName = locationRequest.LocationName;
                }

                await _locationRepo.UpdateLocationRepo(location);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsLocation.LOCATION_UPDATED_SUCCESS;
                res.Data = _mapper.Map<LocationResponse>(location);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Đã xảy ra lỗi khi cập nhật vị trí: {ex.Message}";
                return res;
            }
        }
    }
}
