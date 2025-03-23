using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Exceptions;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.BookingOfflineRepository;
using Repositories.Repositories.OrderRepository;
using Repositories.Repositories.RegisterAttendRepository;
using Repositories.Repositories.WorkShopRepository;
using Services.ApiModels;
using Services.ApiModels.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;
        private readonly IRegisterAttendRepo _registerAttendRepo;
        private readonly IWorkShopRepo _workShopRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBookingOfflineRepo _bookingOfflineRepo;
        public OrderService(IOrderRepo orderRepo, IMapper mapper, IRegisterAttendRepo registerAttendRepo, IWorkShopRepo workShopRepo, IHttpContextAccessor contextAccessor, IBookingOfflineRepo bookingOfflineRepo)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _registerAttendRepo = registerAttendRepo;
            _workShopRepo = workShopRepo;
            _contextAccessor = contextAccessor;
            _bookingOfflineRepo = bookingOfflineRepo;
        }
        public async Task<ResultModel> UpdateOrderToPendingConfirm(string id)
        {
            var res = new ResultModel();
            try
            {
                var order = await _orderRepo.GetOrderById(id);
                if (order == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsOrder.NOT_FOUND;
                    return res;
                }

                order.Status = PaymentStatusEnums.PendingConfirm.ToString();
                order.PaymentReference = null;
                order.PaymentDate = DateTime.Now;
                await _orderRepo.UpdateOrder(order);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsOrder.ORDER_STATUS_TO_PAID;
                return res;
            }
            catch(Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task UpdateBookingOfflineStatusAfterPayment(string serviceId, bool isFirstPayment)
        {
            try
            {
                var bookingOffline = await _bookingOfflineRepo.GetBookingOfflineById(serviceId);
                if (bookingOffline == null)
                    throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE, StatusCodes.Status404NotFound);

                // Xác định trạng thái mới dựa trên lần thanh toán
                string newStatus = isFirstPayment ? BookingOfflineEnums.Paid1st.ToString() : BookingOfflineEnums.Paid2nd.ToString();

                bookingOffline.Status = newStatus;
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating booking offline status: {ex.Message}");
            }
        }

        public async Task<ResultModel> UpdateOrderToPaid(string id)
        {
            var res = new ResultModel();
            try
            {
                var order = await _orderRepo.GetOrderById(id);
                if (order == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsOrder.NOT_FOUND;
                    return res;
                }

                order.Status = PaymentStatusEnums.Paid.ToString();
                order.PaymentReference = null;
                order.PaymentDate = DateTime.Now;
                await _orderRepo.UpdateOrder(order);

                if (order.ServiceType == PaymentTypeEnums.BookingOffline.ToString())
                {
                    bool isFirstPayment = order.Note.Contains("Thanh toán lần 1");
                    await UpdateBookingOfflineStatusAfterPayment(order.ServiceId, isFirstPayment);
                }

                //if(order.ServiceType == PaymentTypeEnums.Course.ToString())
                //{

                //}

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsOrder.ORDER_STATUS_TO_PAID;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetPendingOrders()
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

                var pendingOrders = await _orderRepo.GetOrdersByStatusAndCustomer(PaymentStatusEnums.Pending.ToString(), accountId);

                if (pendingOrders == null || !pendingOrders.Any() || pendingOrders.Count == 0)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsOrder.NOT_FOUND_PENDING;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Data = _mapper.Map<List<OrderResponse>>(pendingOrders);
                res.Message = ResponseMessageConstrantsOrder.FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> CancelOrder(string orderId)
        {
            var res = new ResultModel();
            try
            {
                var order = await _orderRepo.GetOrderById(orderId);
                if (order == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsOrder.NOT_FOUND;
                    return res;
                }

                if (order.ServiceType == PaymentTypeEnums.RegisterAttend.ToString())
                {
                    var tickets = await _registerAttendRepo.GetRegisterAttendsByGroupId(order.ServiceId);
                    if (tickets != null && tickets.Any())
                    {
                        var workshopId = tickets.First().WorkshopId;
                        var workshop = await _workShopRepo.GetWorkShopById(workshopId);

                        if (workshop != null)
                        {
                            workshop.Capacity += tickets.Count;
                            await _workShopRepo.UpdateWorkShop(workshop);
                        }

                        foreach (var ticket in tickets)
                        {
                            await _registerAttendRepo.DeleteRegisterAttend(ticket.AttendId);
                        }
                    }
                }

                order.Status = PaymentStatusEnums.Canceled.ToString();
                order.PaymentReference = null;
                await _orderRepo.UpdateOrder(order);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsOrder.ORDER_CANCELED_SUCCESS;
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

        public async Task<ResultModel> CheckAndUpdateExpiredOrders()
        {
            var res = new ResultModel();
            try
            {
                var pendingOrders = await _orderRepo.GetAllOrders();
                pendingOrders = pendingOrders.Where(o => o.Status == PaymentStatusEnums.Pending.ToString()).ToList();

                if (!pendingOrders.Any())
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Message = ResponseMessageConstrantsOrder.NOT_FOUND_PENDING;
                    return res;
                }

                var expiredOrders = pendingOrders
                    .Where(o => o.CreatedDate.HasValue && o.CreatedDate.Value.AddMinutes(15) < DateTime.Now)
                    .ToList();


                foreach (var order in expiredOrders)
                {
                    if (order.ServiceType == PaymentTypeEnums.RegisterAttend.ToString())
                    {
                        var tickets = await _registerAttendRepo.GetRegisterAttendsByGroupId(order.ServiceId);
                        if (tickets != null && tickets.Any())
                        {
                            var workshopId = tickets.First().WorkshopId;
                            var workshop = await _workShopRepo.GetWorkShopById(workshopId);

                            if (workshop != null)
                            {
                                workshop.Capacity += tickets.Count;
                                await _workShopRepo.UpdateWorkShop(workshop);
                            }
                            foreach (var ticket in tickets)
                            {
                                await _registerAttendRepo.DeleteRegisterAttend(ticket.AttendId);
                            }
                        }
                    }

                    order.Status = PaymentStatusEnums.Expired.ToString();
                    order.PaymentReference = null;
                    await _orderRepo.UpdateOrder(order);
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsOrder.ORDER_EXPIRED;
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

        private string GetAuthenticatedAccountId()
        {
            var identity = _contextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated) return null;

            return identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
