using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Repositories.Repository;
using Services.ApiModels;
using Services.ApiModels.KoiPond;
using Services.ApiModels.Master;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
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
        public async Task<ResultModel<List<MasterListReponseDTO>>> GetAllMasters()
        {
            var res = new ResultModel<List<MasterListReponseDTO>>();
            try
            {
                var masters = await _masterRepo.GetAllMasters();
                var response = _mapper.Map<List<MasterListReponseDTO>>(masters);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = response;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Lỗi khi lấy danh sách thầy phong thủy: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel<MasterDetailReponseDTO>> GetMasterById(string masterId)
        {
            var res = new ResultModel<MasterDetailReponseDTO>();
            try
            {
                if (string.IsNullOrEmpty(masterId))
                {
                    res.IsSuccess = false;
                    res.Message = "ID master không được để trống";
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var master = await _masterRepo.GetByMasterId(masterId);
                if (master == null)
                {
                    res.IsSuccess = false;
                    res.Message = $"Không tìm thấy master với ID: {masterId}";
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                var response = _mapper.Map<MasterDetailReponseDTO>(master);
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = response;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Lỗi khi lấy thông tin master: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }
    }
    }

