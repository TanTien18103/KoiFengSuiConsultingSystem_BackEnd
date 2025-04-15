using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories;
using Services.ApiModels.KoiPond;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.ApiModels.MasterSchedule;
using Services.ApiModels.Master;
using BusinessObjects.Models;
using Repositories.Repositories.AccountRepository;
using Repositories.Repositories.MasterRepository;
using Repositories.Repositories.MasterScheduleRepository;
using BusinessObjects.Constants;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;
using BusinessObjects.Enums;

namespace Services.Services.MasterScheduleService
{
    public class MasterScheduleService : IMasterScheduleService
    {
        private readonly IMasterScheduleRepo _masterScheduleRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IAccountRepo _accountRepo;
        private readonly IMasterRepo _masterRepo;
        public MasterScheduleService(IMasterScheduleRepo masterScheduleRepo, IHttpContextAccessor httpContextAccessor, IMapper mapper, IAccountRepo accountRepo, IMasterRepo masterRepo)
        {
            _masterScheduleRepo = masterScheduleRepo;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _accountRepo = accountRepo;
            _masterRepo = masterRepo;
        }

        public async Task<MasterSchedule> CreateMasterSchedule(MasterSchedule schedule)
        {
            var existingSchedule = await _masterScheduleRepo.GetMasterSchedule(schedule.MasterId, schedule.Date.Value, schedule.StartTime.Value);
            if (existingSchedule != null)
            {
                throw new Exception("Lịch này đã tồn tại!");
            }

            return await _masterScheduleRepo.CreateMasterSchedule(schedule);
        }

        public async Task<ResultModel> GetAllMasterSchedules()
        {
            var res = new ResultModel();
            try
            {
                var masterSchedules = await _masterScheduleRepo.GetAllSchedules();
                if(masterSchedules == null)
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
                res.Data = _mapper.Map<List<MasterSchedulesForMobileResponse>>(masterSchedules);
                res.Message = ResponseMessageConstrantsMasterSchedule.MASTERSCHEDULE_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy danh sách lịch làm việc: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetAllMasterSchedulesForMobile()
        {
            var res = new ResultModel();
            try
            {
                var masterSchedules = await _masterScheduleRepo.GetAllSchedules();
                var InProgressMasterSchedule = masterSchedules.Where(x => x.Status == MasterScheduleEnums.InProgress.ToString()).ToList();
                if (InProgressMasterSchedule == null)
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
                res.Data = _mapper.Map<List<MasterSchedulesForMobileResponse>>(InProgressMasterSchedule);
                res.Message = ResponseMessageConstrantsMasterSchedule.MASTERSCHEDULE_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy danh sách lịch làm việc: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetMasterSchedulesByMaster(string id)
        {
            var res = new ResultModel();
            try
            {
                var master = await _masterRepo.GetByMasterId(id);
                if (master == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsMaster.MASTER_INFO_NOT_FOUND;
                    return res;
                }
                var masterSchedules = await _masterScheduleRepo.GetSchedulesByMasterId(master.MasterId);
                var InProgressMasterSchedule = masterSchedules.Where(x => x.Status == MasterScheduleEnums.InProgress.ToString()).ToList();
                if (InProgressMasterSchedule == null || !InProgressMasterSchedule.Any())
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
                res.Data = _mapper.Map<List<MasterSchedulesForMobileResponse>>(InProgressMasterSchedule);
                res.Message = ResponseMessageConstrantsMasterSchedule.MASTERSCHEDULE_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy danh sách lịch làm việc của thầy: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetMasterSchedulesByCurrentMasterLogin()
        {
            var res = new ResultModel();
            try
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_NOT_SEND;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }
                var token = authHeader.Substring("Bearer ".Length);
                if (string.IsNullOrEmpty(token))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }
                var curMaster = await _accountRepo.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(curMaster))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var master = await _masterRepo.GetMasterByAccountId(curMaster);
                if (master == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsMaster.MASTER_INFO_NOT_FOUND;
                    return res;
                }

                var masterSchedules = await _masterScheduleRepo.GetSchedulesByMasterId(master.MasterId);

                if (masterSchedules == null || !masterSchedules.Any())
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
                res.Data = _mapper.Map<List<MasterSchedulesListDTO>>(masterSchedules);
                res.Message = ResponseMessageConstrantsMasterSchedule.MASTERSCHEDULE_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy danh sách lịch làm việc của thầy: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetMasterSchedulesByMasterAndDate(string masterId, DateTime date)
        {
            var res = new ResultModel();
            try
            {
                var masterSchedules = await _masterScheduleRepo.GetSchedulesByMasterAndDate(masterId, DateOnly.FromDateTime(date));
                if(masterSchedules == null || !masterSchedules.Any())
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
                res.Data = _mapper.Map<List<MasterSchedulesListDTO>>(masterSchedules);
                res.Message = ResponseMessageConstrantsMasterSchedule.MASTERSCHEDULE_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy danh sách lịch làm việc của thầy theo ngày: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }
    }
}
