using AutoMapper;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.ApiModels;
using Services.ApiModels.RegisterAttend;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class RegisterAttendService : IRegisterAttendService
    {
        private readonly IRegisterAttendRepo _registerAttendRepo;
        private readonly IMapper _mapper;
        public RegisterAttendService(IRegisterAttendRepo registerAttendRepo, IMapper mapper)
        {
            _registerAttendRepo = registerAttendRepo;
            _mapper = mapper;
        }

        public async Task<ResultModel> GetRegisterAttends(RegisterAttendStatusEnums? status = null)
        {
            var res = new ResultModel();
            try
            {
                var registerAttends = await _registerAttendRepo.GetRegisterAttends();
                if(registerAttends == null || !registerAttends.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = "Không tìm thấy vé tham dự sự kiện";
                    return res;
                }
                if(status.HasValue)
                {
                    if(status == RegisterAttendStatusEnums.Pending)
                    {
                        registerAttends = registerAttends.Where(x => x.Status == RegisterAttendStatusEnums.Pending.ToString()).ToList();
                    }
                    if (status == RegisterAttendStatusEnums.Confirmed)
                    {
                        registerAttends = registerAttends.Where(x => x.Status == RegisterAttendStatusEnums.Confirmed.ToString()).ToList();
                    }
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<RegisterAttendResponse>>(registerAttends);
                res.Message = "Lấy danh sách vé tham dự sự kiện thành công";
                return res;
            }
            catch(Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }
    }
}
