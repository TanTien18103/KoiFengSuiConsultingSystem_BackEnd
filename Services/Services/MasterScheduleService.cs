using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Repositories.Repository;
using Services.ApiModels.KoiPond;
using Services.ApiModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.ApiModels.MasterSchedule;
using Services.ApiModels.Master;
using BusinessObjects.Models;

namespace Services.Services
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

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<MasterSchedulesListDTO>>(masterSchedules);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Lỗi khi lấy danh sách lịch làm việc: {ex.Message}";
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
                    res.Message = "Token xác thực không được cung cấp";
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }
                var token = authHeader.Substring("Bearer ".Length);
                if (string.IsNullOrEmpty(token))
                {
                    res.IsSuccess = false;
                    res.Message = "Token không hợp lệ";
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }
                var curMaster = await _accountRepo.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(curMaster))
                {
                    res.IsSuccess = false;
                    res.Message = "Token không hợp lệ hoặc đã hết hạn";
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var master = await _masterRepo.GetMasterByAccountId(curMaster);
                if (master == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = "Không tìm thấy thông tin Master";
                    return res;
                }

                var masterSchedules = await _masterScheduleRepo.GetSchedulesByMasterId(master.MasterId);

                if (masterSchedules == null || !masterSchedules.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = "Không tìm thấy schedule";
                    return res;
                }

                var response = _mapper.Map<List<MasterSchedulesListDTO>>(masterSchedules);
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = response;
                res.Message = "Lấy tất cả lịch thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
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
                var response = _mapper.Map<List<MasterSchedulesListDTO>>(masterSchedules);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = response;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Lỗi khi lấy danh sách lịch làm việc của thầy theo ngày: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

    }
}
