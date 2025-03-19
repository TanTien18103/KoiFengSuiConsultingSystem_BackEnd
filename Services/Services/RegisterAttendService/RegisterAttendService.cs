using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.OrderRepository;
using Repositories.Repositories.RegisterAttendRepository;
using Repositories.Repositories.WorkShopRepository;
using Services.ApiModels;
using Services.ApiModels.RegisterAttend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.Services.RegisterAttendService
{
    public class RegisterAttendService : IRegisterAttendService
    {
        private readonly IRegisterAttendRepo _registerAttendRepo;
        private readonly IWorkShopRepo _workShopRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepo _orderRepo;

        public RegisterAttendService(
            IRegisterAttendRepo registerAttendRepo,
            IWorkShopRepo workShopRepo,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IOrderRepo orderRepo)
        {
            _registerAttendRepo = registerAttendRepo;
            _workShopRepo = workShopRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _orderRepo = orderRepo;
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
                res.Data = _mapper.Map<RegisterAttendDetailsResponse>(registerAttend);
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

        public async Task<ResultModel> GetRegisterAttendByGroupId(string groupId)
        {
            var res = new ResultModel();
            try
            {
                var registerAttend = await _registerAttendRepo.GetRegisterAttendsByGroupId(groupId);
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
                res.Data = _mapper.Map<List<RegisterAttendDetailsResponse>>(registerAttend);
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

        public async Task<ResultModel> CreateRegisterAttend(RegisterAttendRequest request)
        {
            var res = new ResultModel();
            try
            {
                if (request == null || request.NumberOfTicket <= 0)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsRegisterAttend.INVALID_TICKET_NUMBER;
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
                var customerId = await _registerAttendRepo.GetCustomerIdByAccountId(accountId);
                var workshop = await _workShopRepo.GetWorkShopById(request.WorkshopId);
                if (workshop == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND;
                    return res;
                }
                if (workshop.StartDate <= DateTime.Now)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsWorkshop.ALREADY_STARTED;
                    return res;
                }

                // Kiểm tra tất cả RegisterAttend của customer
                var allCustomerAttends = await _registerAttendRepo.GetRegisterAttendByCustomerId(customerId);
                
                if (allCustomerAttends.Any())
                {
                    // Lấy RegisterAttend gần nhất của customer
                    var latestRegisterAttend = allCustomerAttends.OrderByDescending(x => x.CreatedDate).First();
                    
                    // Kiểm tra Order của RegisterAttend gần nhất
                    var order = await _orderRepo.GetOrderByServiceId(latestRegisterAttend.GroupId);

                    // Nếu chưa tạo Order hoặc Order chưa thanh toán
                    if (order == null || order.Status != PaymentStatusEnums.Paid.ToString())
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status400BadRequest;
                        res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                        res.Message = "Vui lòng hoàn tất thanh toán cho đăng ký workshop trước đó trước khi đăng ký thêm";
                        return res;
                    }
                }

                if (workshop.Capacity < request.NumberOfTicket)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsWorkshop.CAPACITY_LEFT + workshop.Capacity;
                    return res;
                }
                var registerAttends = new List<RegisterAttend>();
                var createdDate = DateTime.Now;
                var groupId = GenerateShortGuid();
                for (int i = 0; i < request.NumberOfTicket; i++)
                {
                    var registerAttend = new RegisterAttend
                    {
                        AttendId = GenerateShortGuid(),
                        CustomerId = customerId,
                        WorkshopId = request.WorkshopId,
                        Status = RegisterAttendStatusEnums.Pending.ToString(),
                        CreatedDate = createdDate,
                        GroupId = groupId,
                    };
                    var result = await _registerAttendRepo.CreateRegisterAttend(registerAttend);
                    registerAttends.Add(result);
                }
                workshop.Capacity -= request.NumberOfTicket;
                await _workShopRepo.UpdateWorkShop(workshop);

                // Sau khi tạo xong tất cả vé
                var createdTickets = await _registerAttendRepo.GetRegisterAttendsByGroupId(groupId);
                var mappedTickets = createdTickets.Select(ticket => _mapper.Map<RegisterAttendDetailsResponse>(ticket)).ToList();

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Data = new
                {
                    groupId,
                    registerAttends = mappedTickets
                };
                res.Message = ResponseMessageConstrantsRegisterAttend.REGISTERATTEND_CREATED_SUCCESS;
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

        public async Task<ResultModel> UpdatePendingTickets(string workshopId, int newNumberOfTickets)
        {
            var res = new ResultModel();
            try
            {
                if (newNumberOfTickets <= 0)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsRegisterAttend.INVALID_TICKET_NUMBER;
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

                var customerId = await _registerAttendRepo.GetCustomerIdByAccountId(accountId);

                // Lấy workshop
                var workshop = await _workShopRepo.GetWorkShopById(workshopId);
                if (workshop == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND;
                    return res;
                }

                // Kiểm tra workshop đã bắt đầu chưa
                if (workshop.StartDate <= DateTime.Now)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsWorkshop.ALREADY_STARTED;
                    return res;
                }

                var pendingTickets = await _registerAttendRepo.GetPendingTickets(workshopId, customerId);  
                if (!pendingTickets.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsRegisterAttend.PENDING_NOT_FOUND;
                    return res;
                }

                var currentTickets = pendingTickets.Count;
                var ticketDifference = newNumberOfTickets - currentTickets;

                // Kiểm tra capacity nếu tăng số lượng vé
                if (ticketDifference > 0 && workshop.Capacity < ticketDifference)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsWorkshop.CAPACITY_LEFT + workshop.Capacity;
                    return res;
                }

                var groupId = pendingTickets.First().GroupId;

                if (ticketDifference > 0)
                {
                    // Thêm vé mới
                    for (int i = 0; i < ticketDifference; i++)
                    {
                        var newTicket = new RegisterAttend
                        {
                            AttendId = Guid.NewGuid().ToString(),
                            CustomerId = customerId,
                            WorkshopId = workshopId,
                            Status = RegisterAttendStatusEnums.Pending.ToString(),
                            CreatedDate = DateTime.Now,
                            GroupId = groupId,
                        };
                        await _registerAttendRepo.CreateRegisterAttend(newTicket);
                    }
                    workshop.Capacity -= ticketDifference;
                }
                else if (ticketDifference < 0)
                {
                    // Xóa bớt vé
                    var ticketsToRemove = pendingTickets.Take(Math.Abs(ticketDifference)).ToList();
                    foreach (var ticket in ticketsToRemove)
                    {
                        await _registerAttendRepo.DeleteRegisterAttend(ticket.AttendId);
                    }
                    workshop.Capacity += Math.Abs(ticketDifference);
                }

                await _workShopRepo.UpdateWorkShop(workshop);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsRegisterAttend.REGISTERATTEND_UPDATED_SUCCESS;
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

        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }
    }
}
