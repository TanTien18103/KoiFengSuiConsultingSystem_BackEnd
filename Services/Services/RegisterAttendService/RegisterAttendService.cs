using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.RegisterAttendRepository;
using Services.ApiModels;
using Services.ApiModels.RegisterAttend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.RegisterAttendService
{
    public class RegisterAttendService : IRegisterAttendService
    {
        private readonly IRegisterAttendRepo _registerAttendRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public RegisterAttendService(IRegisterAttendRepo registerAttendRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _registerAttendRepo = registerAttendRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetAuthenticatedAccountId()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated) return null;

            return identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<ResultModel> GetRegisterAttends(RegisterAttendStatusEnums? status = null)
        {
            var res = new ResultModel();
            try
            {
                var registerAttends = await _registerAttendRepo.GetRegisterAttends();
                if (registerAttends == null || !registerAttends.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsRegisterAttend.REGISTERATTEND_NOT_FOUND;
                    return res;
                }
                if (status.HasValue)
                {
                    if (status == RegisterAttendStatusEnums.Pending)
                    {
                        registerAttends = registerAttends.Where(x => x.Status == RegisterAttendStatusEnums.Pending.ToString()).ToList();
                    }
                    if (status == RegisterAttendStatusEnums.Confirmed)
                    {
                        registerAttends = registerAttends.Where(x => x.Status == RegisterAttendStatusEnums.Confirmed.ToString()).ToList();
                    }
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<RegisterAttendResponse>>(registerAttends);
                res.Message = ResponseMessageConstrantsRegisterAttend.REGISTERATTEND_FOUND;
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
        public async Task<ResultModel> GetRegisterAttendByCustomerId()
        {
            var res = new ResultModel();
            try
            {
                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }
                var customerId = await _registerAttendRepo.GetCustomerIdByAccountId(accountId);
                var registerAttends = await _registerAttendRepo.GetRegisterAttendByCustomerId(customerId);
                if (registerAttends == null || !registerAttends.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsRegisterAttend.REGISTERATTEND_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Data = _mapper.Map<List<RegisterAttendCustomerResponse>>(registerAttends);
                res.Message = ResponseMessageConstrantsRegisterAttend.REGISTERATTEND_FOUND;
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

        public async Task<ResultModel> GetRegisterAttendById(string registerAttendId)
        {
            var res = new ResultModel();
            try
            {
                var registerAttend = await _registerAttendRepo.GetRegisterAttendById(registerAttendId);
                if (registerAttend == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsRegisterAttend.REGISTERATTEND_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<RegisterAttendResponse>(registerAttend);
                res.Message = ResponseMessageConstrantsRegisterAttend.REGISTERATTEND_FOUND;
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
        public async Task<ResultModel> GetRegisterAttends()
        {
            var res = new ResultModel();
            try
            {
                var registerAttends = await _registerAttendRepo.GetRegisterAttends();
                if (registerAttends == null || !registerAttends.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsRegisterAttend.REGISTERATTEND_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<RegisterAttendResponse>>(registerAttends);
                res.Message = ResponseMessageConstrantsRegisterAttend.REGISTERATTEND_FOUND;
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


        public async Task<ResultModel> GetRegisterAttendByWorkshopId(string id)
        {
            var res = new ResultModel();
            try
            {
                var registerAttends = await _registerAttendRepo.GetRegisterAttendsByWorkShopId(id);
                if (registerAttends == null || !registerAttends.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsRegisterAttend.REGISTERATTEND_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Data = _mapper.Map<List<RegisterAttendResponse>>(registerAttends);
                res.Message = ResponseMessageConstrantsRegisterAttend.REGISTERATTEND_FOUND;
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
