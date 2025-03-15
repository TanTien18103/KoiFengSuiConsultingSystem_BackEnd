using AutoMapper;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.WorkShopRepository;
using Repositories.Repositories;
using Services.ApiModels;
using Services.ApiModels.Workshop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Constants;
using Repositories.Repositories.RegisterAttendRepository;

namespace Services.Services.WorkshopService
{
    public class WorkshopService : IWorkshopService
    {
        private readonly IWorkShopRepo _workShopRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRegisterAttendRepo _registerAttendRepo;

        public WorkshopService(IWorkShopRepo workShopRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor, IRegisterAttendRepo registerAttendRepo)
        {
            _workShopRepo = workShopRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _registerAttendRepo = registerAttendRepo;
        }


        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }

        public async Task<ResultModel> SortingWorkshopByCreatedDate()
        {
            var res = new ResultModel();
            try
            {
                var workshops = await _workShopRepo.SortingWorkshopByCreatedDate();
                if (workshops == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<WorkshopResponse>>(workshops);
                res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_FOUND;
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
        public async Task<ResultModel> ApprovedWorkshop(string id)
        {
            var res = new ResultModel();
            try
            {
                var workshop = await _workShopRepo.GetWorkShopById(id);
                if (workshop == null || workshop.Status != WorkshopStatusEnums.Pending.ToString())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_PENDING_NOT_FOUND;
                    return res;
                }
                if (workshop != null && workshop.Status == WorkshopStatusEnums.Pending.ToString())
                {
                    workshop.Status = WorkshopStatusEnums.Approved.ToString();
                    await _workShopRepo.UpdateWorkShop(workshop);
                }
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<WorkshopResponse>(workshop);
                res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_APPROVED;
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
        public async Task<ResultModel> RejectedWorkshop(string id)
        {
            var res = new ResultModel();
            try
            {
                var workshop = await _workShopRepo.GetWorkShopById(id);
                if (workshop == null || workshop.Status != WorkshopStatusEnums.Pending.ToString())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_PENDING_NOT_FOUND;
                    return res;
                }
                if (workshop != null && workshop.Status == WorkshopStatusEnums.Pending.ToString())
                {
                    workshop.Status = WorkshopStatusEnums.Rejected.ToString();
                    await _workShopRepo.UpdateWorkShop(workshop);
                }
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<WorkshopResponse>(workshop);
                res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_REJECTED;
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
        public async Task<ResultModel> TrendingWorkshop(bool? trending = null)
        {
            var res = new ResultModel();
            try
            {
                var workshops = await _workShopRepo.GetWorkShops();
                if (workshops == null || !workshops.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND;
                    return res;
                }

                if (trending == true)
                {
                    workshops = workshops.Where(x => x.Trending == trending).ToList();
                }
                if (trending == false)
                {
                    workshops = workshops.Where(x => x.Trending == trending).ToList();
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<WorkshopResponse>>(workshops);
                res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_FOUND;
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

        public async Task<ResultModel> GetWorkshopById(string id)
        {
            var res = new ResultModel();

            try
            {
                var workshop = await _workShopRepo.GetWorkShopById(id);
                if (workshop == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<WorkshopResponse>(workshop);
                res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_INFO_FOUND;
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

        public async Task<ResultModel> CreateWorkshop(WorkshopRequest request)
        {
            var res = new ResultModel();

            try
            {
                if (request == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_INFO_INVALID;
                    return res;
                }

                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }

                var masterId = await _workShopRepo.GetMasterIdByAccountId(accountId);
                if (masterId == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.NOTFOUND_MASTERID_CORRESPONDING_TO_ACCOUNT;
                    return res;
                }

                var existingWorkshop = await _workShopRepo.GetWorkshopByMasterLocationAndDate(masterId, request.Location, request.StartDate);
                if (existingWorkshop != null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.EXISTED;
                    res.StatusCode = StatusCodes.Status409Conflict;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_DUPLICATE_LOCATION_DATE_SAME_MASTER;
                    return res;
                }

                var existingWorkshopOtherMaster = await _workShopRepo.GetWorkshopByLocationAndDate(request.Location, request.StartDate);
                if (existingWorkshopOtherMaster != null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.EXISTED;
                    res.StatusCode = StatusCodes.Status409Conflict;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_DUPLICATE_LOCATION_DATE_OTHER_MASTER;
                    return res;
                }

                var workshopsByMaster = await _workShopRepo.GetWorkshopsByMaster(masterId);
                foreach (var ws in workshopsByMaster)
                {
                    if (ws.Location != request.Location && request.StartDate.HasValue && ws.StartDate.HasValue)
                    {
                        var timeDifference = (request.StartDate.Value - ws.StartDate.Value).TotalHours;
                        if (Math.Abs(timeDifference) < 5)
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.EXISTED;
                            res.StatusCode = StatusCodes.Status409Conflict;
                            res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_MINIMUM_HOURS_DIFFERENCE;
                            return res;
                        }
                    }
                }

                var newWorkshop = _mapper.Map<WorkShop>(request);
                newWorkshop.WorkshopId = GenerateShortGuid();
                newWorkshop.CreatedDate = DateTime.UtcNow;
                newWorkshop.Status = WorkshopStatusEnums.Pending.ToString();
                newWorkshop.MasterId = masterId;

                if (newWorkshop.MasterId == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.NOTFOUND_MASTERID_CORRESPONDING_TO_ACCOUNT;
                    return res;
                }

                await _workShopRepo.CreateWorkShop(newWorkshop);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Data = _mapper.Map<WorkshopResponse>(newWorkshop);
                res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_CREATED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi tạo hội thảo: {ex.Message}";
                return res;
            }
        }

        private string GetAuthenticatedAccountId()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated) return null;

            return identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<ResultModel> UpdateWorkshop(string id, WorkshopRequest request)
        {
            var res = new ResultModel();
            try
            {
                var workshop = await _workShopRepo.GetWorkShopById(id);
                if (workshop == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND;
                    return res;
                }

                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }

                var masterId = await _workShopRepo.GetMasterIdByAccountId(accountId);
                if (masterId == null || masterId != workshop.MasterId)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_UPDATE_NOT_ALLOWED;
                    return res;
                }

                _mapper.Map(request, workshop);

                await _workShopRepo.UpdateWorkShop(workshop);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<WorkshopResponse>(workshop);
                res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_UPDATED_SUCCESS;
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

        public async Task<ResultModel> DeleteWorkshop(string id)
        {
            var res = new ResultModel();
            try
            {
                var workshop = await _workShopRepo.GetWorkShopById(id);
                if (workshop == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND;
                    return res;
                }

                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }

                var masterId = await _workShopRepo.GetMasterIdByAccountId(accountId);
                if (masterId == null || masterId != workshop.MasterId)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_DELETE_NOT_ALLOWED;
                    return res;
                }

                await _workShopRepo.DeleteWorkShop(id);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_DELETED_SUCCESS;
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

        public async Task<ResultModel> CheckIn(string workshopId, string registerId)
        {
            var res = new ResultModel();
            try
            {
                var workshop = await _workShopRepo.GetWorkShopById(workshopId);
                if (workshop == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND;
                    return res;
                }

                var register = await _registerAttendRepo.GetRegisterAttendById(registerId);
                if (register == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.REGISTER_NOT_FOUND;
                    return res;
                }

                register.Status = RegisterAttendStatusEnums.Confirmed.ToString(); 
                await _registerAttendRepo.UpdateRegisterAttend(register);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsWorkshop.CHECK_IN_SUCCESS;
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
