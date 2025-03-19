using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.MasterRepository;
using Repositories.Repositories;
using Services.ApiModels;
using Services.ApiModels.KoiPond;
using Services.ApiModels.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.Services.MasterService
{
    public class MasterService : IMasterService
    {
        private readonly IMasterRepo _masterRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public MasterService(IMasterRepo masterRepo, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _masterRepo = masterRepo;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<ResultModel> GetAllMasters()
        {
            var res = new ResultModel();
            try
            {
                var masters = await _masterRepo.GetAllMasters();
                if(masters == null || !masters.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsMaster.MASTER_NOT_FOUND;
                    return res;
                }
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<MasterListReponseDTO>>(masters);
                res.Message = ResponseMessageConstrantsMaster.MASTER_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy danh sách master: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetMasterById(string masterId)
        {
            var res = new ResultModel();
            try
            {
                var master = await _masterRepo.GetByMasterId(masterId);
                if (master == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsMaster.MASTER_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<MasterListReponseDTO>(master);
                res.Message = ResponseMessageConstrantsMaster.MASTER_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy thông tin master: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }
    }
}

