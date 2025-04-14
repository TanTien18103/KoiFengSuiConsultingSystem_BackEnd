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
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;
using Repositories.Repositories.MasterRepository;
using Services.ServicesHelpers.UploadService;
using Repositories.Repositories.MasterScheduleRepository;
using Repositories.Repositories.LocationRepository;
using System.Net.WebSockets;

namespace Services.Services.WorkshopService
{
    public class WorkshopService : IWorkshopService
    {
        private readonly IWorkShopRepo _workShopRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRegisterAttendRepo _registerAttendRepo;
        private readonly IMasterRepo _masterRepo;
        private readonly IUploadService _uploadService;
        private readonly IMasterScheduleRepo _masterScheduleRepo;
        private readonly ILocationRepo _locationRepo;

        public string? WORKSHOP_DUPLICATE_SCHEDULE_SAME_MASTER { get; private set; }

        public WorkshopService(IWorkShopRepo workShopRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IRegisterAttendRepo registerAttendRepo, IMasterRepo masterRepo, IUploadService uploadService,
            IMasterScheduleRepo masterScheduleRepo, ILocationRepo locationRepo)
        {
            _workShopRepo = workShopRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _registerAttendRepo = registerAttendRepo;
            _masterRepo = masterRepo;
            _uploadService = uploadService;
            _masterScheduleRepo = masterScheduleRepo;
            _locationRepo = locationRepo;
        }
        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }
        private bool IsTimeOverlap(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
        {
            return startA < endB && endA > startB;
        }
        private string GetAuthenticatedAccountId()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated) return null;

            return identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<ResultModel> SortingWorkshopByCreatedDate()
        {
            var res = new ResultModel();
            try
            {
                var workshops = await _workShopRepo.SortingWorkshopByCreatedDate();
                var approvedWorkshops = workshops.Where(x => x.Status == WorkshopStatusEnums.Approved.ToString()).ToList();
                if (approvedWorkshops == null)
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
                res.Data = _mapper.Map<List<WorkshopResponse>>(approvedWorkshops);
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

        public async Task<ResultModel> SortingWorkshopByCreatedDateForWeb()
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
                var masterByWorkshop = await _masterRepo.GetMasterByWorkshopId(workshop.WorkshopId);
                var masterSchedule = await _masterScheduleRepo.GetSchedulesByMasterId(masterByWorkshop.MasterId);

                
                if(masterSchedule == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsMasterSchedule.MASTERSCHEDULE_NOT_FOUND;
                    return res;
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
        public async Task<ResultModel> TrendingWorkshop()
        {
            var res = new ResultModel();
            try
            {
                var workshops = await _workShopRepo.GetWorkShops();
                var approvedWorkshops = workshops.Where(x => x.Status == WorkshopStatusEnums.Approved.ToString()).ToList();
                if (approvedWorkshops == null || !approvedWorkshops.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND;
                    return res;
                }
                bool trending = true;
                if (trending)
                {
                    approvedWorkshops = approvedWorkshops.Where(x => x.Trending == trending).ToList();
                    if (!approvedWorkshops.Any() || approvedWorkshops == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND;
                        return res;
                    }
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<WorkshopResponse>>(approvedWorkshops);
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

                if (!TimeOnly.TryParse(request.StartTime, out var startTime))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsWorkshop.STARTTIME_INFO_INVALID;
                    return res;
                }

                if (!TimeOnly.TryParse(request.EndTime, out var endTime))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsWorkshop.ENDTIME_INFO_INVALID;
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

                var location = await _locationRepo.GetLocationByIdRepo(request.LocationId);
                if (location == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.LOCATION_NOT_FOUND;
                    return res;
                }

                var masterId = await _masterRepo.GetMasterIdByAccountId(accountId);
                if (masterId == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.NOTFOUND_MASTERID_CORRESPONDING_TO_ACCOUNT;
                    return res;
                }

                if (endTime <= startTime)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsWorkshop.TIME_INVALID;
                    return res;
                }

                // Lấy tất cả workshop trùng ngày
                var workshopsInSameDate = await _workShopRepo.GetWorkshopsByDate(request.StartDate.Value);

                // Convert thời gian để so sánh
                var newStart = request.StartDate.Value.Add(startTime.ToTimeSpan());
                var newEnd = request.StartDate.Value.Add(endTime.ToTimeSpan());

                var duration = newEnd - newStart;
                if (duration.TotalMinutes < 30 || duration.TotalHours > 3)
                {
                    res.IsSuccess = false;
                    res.Message = ResponseMessageConstrantsWorkshop.DURATION_INVALID;
                    return res;
                }

                // Lặp qua các workshop
                foreach (var ws in workshopsInSameDate)
                {
                    var existingStart = ws.StartDate.Value.Add(ws.StartTime!.Value.ToTimeSpan());
                    var existingEnd = ws.StartDate.Value.Add(ws.EndTime!.Value.ToTimeSpan());
                    double gapHours = Math.Abs((newStart - existingStart).TotalHours);

                    // 1. Nếu cùng location cùng master
                    if (ws.LocationId == request.LocationId && ws.MasterId == masterId)
                    {
                        if (gapHours < 1 || IsTimeOverlap(newStart, newEnd, existingStart, existingEnd))
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.EXISTED;
                            res.StatusCode = StatusCodes.Status409Conflict;
                            res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_DUPLICATE_LOCATION;
                            return res;
                        }
                    }
                    // 2. Nếu khác location nhưng cùng master
                    else if (ws.MasterId == masterId)
                    {
                        if (gapHours < 1 || IsTimeOverlap(newStart, newEnd, existingStart, existingEnd))
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.EXISTED;
                            res.StatusCode = StatusCodes.Status409Conflict;
                            res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_DUPLICATE_SCHEDULE_SAME_MASTER;
                            return res;
                        }
                    }
                    // 3. Nếu cùng location nhưng khác master
                    else if (ws.LocationId == request.LocationId && ws.MasterId != masterId)
                    {
                        if (gapHours < 1 || IsTimeOverlap(newStart, newEnd, existingStart, existingEnd))
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.EXISTED;
                            res.StatusCode = StatusCodes.Status409Conflict;
                            res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_DUPLICATE_LOCATION_DATE_OTHER_MASTER;
                            return res;
                        }
                    }
                }
               
                var masterschedules = await _masterScheduleRepo.GetMasterScheduleByMasterId(masterId);

                foreach (var masterschedule in masterschedules)
                {
                    if (masterschedule.Date == DateOnly.FromDateTime(request.StartDate.Value) && masterschedule.StartTime == startTime && masterschedule.EndTime == endTime)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.EXISTED;
                        res.Message = ResponseMessageConstrantsMasterSchedule.MASTERSCHEDULE_EXISTED_SLOT;
                        res.StatusCode = StatusCodes.Status409Conflict;
                        return res;
                    }
                }

                var masterSchedule = new MasterSchedule
                {
                    MasterScheduleId = GenerateShortGuid(),
                    MasterId = masterId,
                    Date = DateOnly.FromDateTime(request.StartDate.Value),
                    StartTime = startTime,
                    EndTime = endTime,
                    Type = MasterScheduleTypeEnums.Workshop.ToString(),
                    Status = MasterScheduleEnums.Pending.ToString(),
                    CreateDate = DateTime.UtcNow,
                };
                var createmasterSchedule = await _masterScheduleRepo.CreateMasterSchedule(masterSchedule);

                var newWorkshop = _mapper.Map<WorkShop>(request);
                newWorkshop.WorkshopId = GenerateShortGuid();
                newWorkshop.CreatedDate = DateTime.UtcNow;
                newWorkshop.Status = WorkshopStatusEnums.Pending.ToString();
                newWorkshop.MasterId = masterId;
                newWorkshop.StartTime = startTime;
                newWorkshop.EndTime = endTime;
                newWorkshop.MasterScheduleId = createmasterSchedule.MasterScheduleId;
                newWorkshop.ImageUrl = await _uploadService.UploadImageAsync(request.ImageUrl);

                if (newWorkshop.MasterId == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.NOTFOUND_MASTERID_CORRESPONDING_TO_ACCOUNT;
                    return res;
                }

                await _workShopRepo.CreateWorkShop(newWorkshop);

                var workshopResponse = await _workShopRepo.GetWorkShopById(newWorkshop.WorkshopId);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Data = _mapper.Map<WorkshopResponse>(workshopResponse);
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

        public async Task<ResultModel> UpdateWorkshop(string id, WorkshopUpdateRequest request)
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

                var masterId = await _masterRepo.GetMasterIdByAccountId(accountId);
                if (masterId == null || masterId != workshop.MasterId)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_UPDATE_NOT_ALLOWED;
                    return res;
                }

                if (!TimeOnly.TryParse(request.StartTime, out var startTime))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsWorkshop.STARTTIME_INFO_INVALID;
                    return res;
                }

                if (!TimeOnly.TryParse(request.EndTime, out var endTime))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsWorkshop.ENDTIME_INFO_INVALID;
                    return res;
                }

                // Partial update
                if (!string.IsNullOrEmpty(request.WorkshopName))
                    workshop.WorkshopName = request.WorkshopName;

                if (request.StartDate.HasValue)
                    workshop.StartDate = request.StartDate.Value;

                if (!string.IsNullOrEmpty(request.LocationId))
                    workshop.LocationId = request.LocationId;

                if (!string.IsNullOrEmpty(request.Description))
                    workshop.Description = request.Description;

                if (request.Capacity.HasValue)
                    workshop.Capacity = request.Capacity.Value;

                if (request.Capacity.HasValue)
                    workshop.Capacity = request.Capacity.Value;

                if (!string.IsNullOrEmpty(request.StartTime))
                    workshop.StartTime = startTime;

                if (!string.IsNullOrEmpty(request.EndTime))
                    workshop.EndTime = endTime;

                if (request.Price.HasValue)
                    workshop.Price = request.Price.Value;

                if (request.ImageUrl != null)
                    workshop.ImageUrl = await _uploadService.UploadImageAsync(request.ImageUrl);

                if (endTime <= startTime)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsWorkshop.TIME_INVALID;
                    return res;
                }

                var workshopsInSameDate = await _workShopRepo.GetWorkshopsByDate(request.StartDate.Value);

                // Convert thời gian để so sánh
                var newStart = request.StartDate.Value.Add(startTime.ToTimeSpan());
                var newEnd = request.StartDate.Value.Add(endTime.ToTimeSpan());

                var duration = newEnd - newStart;
                if (duration.TotalMinutes < 30 || duration.TotalHours > 3)
                {
                    res.IsSuccess = false;
                    res.Message = ResponseMessageConstrantsWorkshop.DURATION_INVALID;
                    return res;
                }

                // Lặp qua các workshop
                foreach (var ws in workshopsInSameDate)
                {
                    var existingStart = ws.StartDate.Value.Add(ws.StartTime!.Value.ToTimeSpan());
                    var existingEnd = ws.StartDate.Value.Add(ws.EndTime!.Value.ToTimeSpan());
                    double gapHours = Math.Abs((newStart - existingStart).TotalHours);

                    // 1. Nếu cùng location cùng master
                    if (ws.LocationId == request.LocationId && ws.MasterId == masterId)
                    {
                        if (gapHours < 1 || IsTimeOverlap(newStart, newEnd, existingStart, existingEnd))
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.EXISTED;
                            res.StatusCode = StatusCodes.Status409Conflict;
                            res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_DUPLICATE_LOCATION;
                            return res;
                        }
                    }
                    // 2. Nếu khác location nhưng cùng master
                    else if (ws.MasterId == masterId)
                    {
                        if (gapHours < 1 || IsTimeOverlap(newStart, newEnd, existingStart, existingEnd))
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.EXISTED;
                            res.StatusCode = StatusCodes.Status409Conflict;
                            res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_DUPLICATE_SCHEDULE_SAME_MASTER;
                            return res;
                        }
                    }
                    // 3. Nếu cùng location nhưng khác master
                    else if (ws.LocationId == request.LocationId && ws.MasterId != masterId)
                    {
                        if (gapHours < 1 || IsTimeOverlap(newStart, newEnd, existingStart, existingEnd))
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.EXISTED;
                            res.StatusCode = StatusCodes.Status409Conflict;
                            res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_DUPLICATE_LOCATION_DATE_OTHER_MASTER;
                            return res;
                        }
                    }
                }
                var Date = DateOnly.FromDateTime(request.StartDate.Value);

                var masterschedule = await _masterScheduleRepo.GetMasterScheduleById(workshop.MasterScheduleId);

                var masterSchedule = new MasterSchedule
                {
                    MasterScheduleId = masterschedule.MasterScheduleId,
                    MasterId = masterId,
                    Date = Date,
                    Type = MasterScheduleTypeEnums.Workshop.ToString(),
                    StartTime = startTime,
                    EndTime = endTime,
                    Status = MasterScheduleEnums.Pending.ToString(),
                    UpdateDate = DateTime.UtcNow,
                };

                var masterschedules = await _masterScheduleRepo.GetMasterScheduleByMasterId(masterId);

                foreach (var ms in masterschedules)
                {
                    if (ms.Date == masterSchedule.Date.GetValueOrDefault() && ms.StartTime == startTime && ms.EndTime == endTime)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.EXISTED;
                        res.Message = ResponseMessageConstrantsMasterSchedule.MASTERSCHEDULE_EXISTED_SLOT;
                        res.StatusCode = StatusCodes.Status409Conflict;
                        return res;
                    }
                }

                var createmasterSchedule = await _masterScheduleRepo.UpdateMasterSchedule(masterSchedule);

                workshop.UpdateAt = DateTime.UtcNow;
                workshop.StartTime = startTime;
                workshop.EndTime = endTime;
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

                var masterId = await _masterRepo.GetMasterIdByAccountId(accountId);
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

        public async Task<ResultModel> CancelWorkshop()
        {
            var res = new ResultModel();
            try
            {
                var workshops = await _workShopRepo.GetWorkShops();
                var approvedWorkshops = workshops.Where(x => x.Status == WorkshopStatusEnums.Approved.ToString()).ToList();

                if (approvedWorkshops == null || !approvedWorkshops.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND;
                    return res;
                }

                foreach (var workshop in approvedWorkshops)
                {
                    if (workshop.StartDate.HasValue)
                    {
                        var oneDayBefore = workshop.StartDate.Value.AddDays(-1);
                        if (DateTime.UtcNow >= oneDayBefore && workshop.StartDate > DateTime.UtcNow)
                        {
                            var registerAttends = await _registerAttendRepo.GetRegisterAttendsByWorkShopId(workshop.WorkshopId);
                            if (registerAttends == null || !registerAttends.Any())
                            {
                                workshop.Status = WorkshopStatusEnums.Canceled.ToString();
                                await _workShopRepo.UpdateWorkShop(workshop);

                                var masterSchedules = await _masterScheduleRepo.GetMasterScheduleByMasterId(workshop.MasterId);
                                var scheduleToUpdate = masterSchedules.FirstOrDefault(ms =>
                                    ms.Date == DateOnly.FromDateTime(workshop.StartDate.Value) &&
                                    ms.Type == MasterScheduleTypeEnums.Workshop.ToString());

                                if (scheduleToUpdate != null)
                                {
                                    scheduleToUpdate.Status = MasterScheduleEnums.Canceled.ToString();
                                    await _masterScheduleRepo.UpdateMasterSchedule(scheduleToUpdate);
                                }
                            }
                        }
                    }
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_CANCELED_SUCCESS;
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
