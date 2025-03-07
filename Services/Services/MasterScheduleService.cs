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
        public MasterScheduleService(IMasterScheduleRepo masterScheduleRepo, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _masterScheduleRepo = masterScheduleRepo;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
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

        public async Task<ResultModel> GetMasterSchedulesByMasterId(string masterId)
        {
            var res = new ResultModel();
            try
            {
                var masterSchedules = await _masterScheduleRepo.GetSchedulesByMasterId(masterId);
                var response = _mapper.Map<List<MasterSchedulesListDTO>>(masterSchedules);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = response;
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
