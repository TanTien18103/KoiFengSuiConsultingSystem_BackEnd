using AutoMapper;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Repositories.Repository;
using Services.ApiModels;
using Services.ApiModels.Workshop;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class WorkshopService : IWorkshopService
    {
        private readonly IWorkShopRepo _workShopRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WorkshopService(IWorkShopRepo workShopRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _workShopRepo = workShopRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
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
                if(workshops == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy buổi hội thảo nào";
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK; 
                res.Data = _mapper.Map<List<WorkshopResponse>>(workshops);
                res.Message = "Lấy danh sách buổi hội thảo thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
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
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy buổi hội thảo đang chờ được cập nhật";
                    return res;
                }
                if(workshop != null && workshop.Status == WorkshopStatusEnums.Pending.ToString())
                {
                    workshop.Status = WorkshopStatusEnums.Approved.ToString();
                    await _workShopRepo.UpdateWorkShop(workshop);
                }
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<WorkshopResponse>(workshop);
                res.Message = "Buổi hội thảo được phê duyệt";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
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
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy buổi hội thảo đang chờ được cập nhật";
                    return res;
                }
                if (workshop != null && workshop.Status == WorkshopStatusEnums.Pending.ToString())
                {
                    workshop.Status = WorkshopStatusEnums.Rejected.ToString();
                    await _workShopRepo.UpdateWorkShop(workshop);
                }
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<WorkshopResponse>(workshop);
                res.Message = "Buổi hội thảo bị từ chối";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
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
                if(workshops == null || !workshops.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy buổi hội thảo nào";
                    return res;
                }

                if(trending == true)
                {
                    workshops = workshops.Where(x => x.Trending == trending).ToList();
                }
                if(trending == false)
                {
                    workshops = workshops.Where(x => x.Trending == trending).ToList();
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<WorkshopResponse>>(workshops);
                res.Message = "Lấy danh sách buổi hội thảo thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetWorkshop()
        {
            var res = new ResultModel();
            try
            {
                var workshops = await _workShopRepo.GetWorkShops();
                if (workshops == null || !workshops.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy buổi hội thảo nào";
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<WorkshopResponse>>(workshops);
                res.Message = "Lấy danh sách buổi hội thảo thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
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
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy buổi hội thảo";
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<WorkshopResponse>(workshop);
                res.Message = "Lấy thông tin buổi hội thảo thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
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
                // Validate request
                if (request == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu không hợp lệ"
                    };
                }

                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "Người dùng chưa xác thực hoặc không có quyền truy cập"
                    };
                }

                var newWorkshop = _mapper.Map<WorkShop>(request);
                newWorkshop.WorkshopId = GenerateShortGuid();
                newWorkshop.CreatedDate = DateTime.UtcNow;
                newWorkshop.Status = WorkshopStatusEnums.Pending.ToString();
                newWorkshop.MasterId = await _workShopRepo.GetMasterIdByAccountId(accountId);

                if (newWorkshop.MasterId == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Không tìm thấy MasterId tương ứng với tài khoản"
                    };
                }

                await _workShopRepo.CreateWorkShop(newWorkshop);

                return new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status201Created,
                    Data = _mapper.Map<WorkshopResponse>(newWorkshop),
                    Message = "Buổi hội thảo đã được tạo thành công"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Đã xảy ra lỗi khi tạo hội thảo. Vui lòng thử lại sau."
                };
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
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = "Không tìm thấy buổi hội thảo";
                    return res;
                }

                _mapper.Map(request, workshop);

                await _workShopRepo.UpdateWorkShop(workshop);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<WorkshopResponse>(workshop);
                res.Message = "Cập nhật buổi hội thảo thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
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
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = "Không tìm thấy buổi hội thảo";
                    return res;
                }

                await _workShopRepo.DeleteWorkShop(id);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = "Xóa buổi hội thảo thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

    }
}
    