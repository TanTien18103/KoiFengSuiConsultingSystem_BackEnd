using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Exceptions;
using BusinessObjects.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.AnswerRepository;
using Repositories.Repositories.BookingOfflineRepository;
using Repositories.Repositories.BookingOnlineRepository;
using Repositories.Repositories.ChapterRepository;
using Repositories.Repositories.CourseRepository;
using Repositories.Repositories.EnrollAnswerRepository;
using Repositories.Repositories.EnrollCertRepository;
using Repositories.Repositories.EnrollChapterRepository;
using Repositories.Repositories.EnrollQuizRepository;
using Repositories.Repositories.MasterScheduleRepository;
using Repositories.Repositories.OrderRepository;
using Repositories.Repositories.QuestionRepository;
using Repositories.Repositories.QuizRepository;
using Repositories.Repositories.RegisterAttendRepository;
using Repositories.Repositories.RegisterCourseRepository;
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
        private readonly IBookingOnlineRepo _bookingOnlineRepo;
        private readonly ICourseRepo _courseRepo;
        private readonly IChapterRepo _chapterRepo;
        private readonly IRegisterCourseRepo _registerCourseRepo;
        private readonly IEnrollChapterRepo _enrollChapterRepo;
        private readonly IMasterScheduleRepo _masterScheduleRepo;

        public OrderService(IOrderRepo orderRepo, IMapper mapper, IRegisterAttendRepo registerAttendRepo, IWorkShopRepo workShopRepo, IHttpContextAccessor contextAccessor, IBookingOfflineRepo bookingOfflineRepo, ICourseRepo courseRepo, IChapterRepo chapterRepo, IRegisterCourseRepo registerCourseRepo, IEnrollChapterRepo enrollChapterRepo, IBookingOnlineRepo bookingOnlineRepo, IMasterScheduleRepo masterScheduleRepo)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _registerAttendRepo = registerAttendRepo;
            _workShopRepo = workShopRepo;
            _contextAccessor = contextAccessor;
            _bookingOfflineRepo = bookingOfflineRepo;
            _courseRepo = courseRepo;
            _chapterRepo = chapterRepo;
            _registerCourseRepo = registerCourseRepo;
            _enrollChapterRepo = enrollChapterRepo;
            _bookingOnlineRepo = bookingOnlineRepo;
            _masterScheduleRepo = masterScheduleRepo;
        }

        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
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

                if (order.ServiceType == PaymentTypeEnums.BookingOffline.ToString())
                {
                    var bookingOffline = await _bookingOfflineRepo.GetBookingOfflineById(order.ServiceId);
                    if (bookingOffline == null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                        return res;
                    }

                    bool isFirstPayment = order.Note.Contains("Thanh toán đặt cọc 30%");
                    bookingOffline.Status = isFirstPayment ?
                        BookingOfflineEnums.FirstPaymentPending.ToString() :
                        BookingOfflineEnums.SecondPaymentPending.ToString();

                    await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);
                }

                else if (order.ServiceType == PaymentTypeEnums.BookingOnline.ToString())
                {
                    var bookingOnline = await _bookingOnlineRepo.GetBookingOnlineByIdRepo(order.ServiceId);
                    if (bookingOnline == null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_ONLINE;
                        return res;
                    }

                    bookingOnline.Status = BookingOnlineEnums.PendingConfirm.ToString();
                    await _bookingOnlineRepo.UpdateBookingOnlineRepo(bookingOnline);
                }

                else if (order.ServiceType == PaymentTypeEnums.RegisterAttend.ToString())
                {
                    var registerAttends = await _registerAttendRepo.GetRegisterAttendsByGroupId(order.ServiceId);
                    if (registerAttends == null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_ONLINE;
                        return res;
                    }
                    foreach (var attend in registerAttends)
                    {
                        attend.Status = BookingOnlineEnums.PendingConfirm.ToString();
                        await _registerAttendRepo.UpdateRegisterAttend(attend);
                    }
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
            catch (Exception ex)
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
                string newStatus = isFirstPayment ? BookingOfflineEnums.FirstPaymentSuccess.ToString() : BookingOfflineEnums.Completed.ToString();

                bookingOffline.Status = newStatus;
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);
            }
            catch (Exception ex)
            {
                throw new AppException(ResponseCodeConstants.FAILED, ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultModel> UpdateOrderToPaid(string orderId)
        {
            var res = new ResultModel();
            try
            {
                var order = await _orderRepo.GetOrderById(orderId);
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

                // RegisterAttend
                if (order.ServiceType == PaymentTypeEnums.RegisterAttend.ToString())
                {
                    var tickets = await _registerAttendRepo.GetRegisterAttendsByGroupId(order.ServiceId);
                    var paidOrders = await _orderRepo.GetAllOrders();
                    var paidOrder = paidOrders.FirstOrDefault(x => x.Status == PaymentStatusEnums.Paid.ToString());
                    if (paidOrder != null)
                    {
                        foreach (var ticket in tickets)
                        {
                            ticket.Status = PaymentStatusEnums.Paid.ToString();
                            await _registerAttendRepo.UpdateRegisterAttend(ticket);
                        }
                    }
                    if (tickets != null && tickets.Any())
                    {
                        var workshopId = tickets.First().WorkshopId;
                        var workshop = await _workShopRepo.GetWorkShopById(workshopId);

                        if (workshop != null)
                        {
                            workshop.Capacity -= tickets.Count;
                            await _workShopRepo.UpdateWorkShop(workshop);

                            if (workshop.Capacity == 0)
                            {
                                var pendingTickets = await _registerAttendRepo.GetRegisterAttendsByWorkShopId(workshopId);
                                if (pendingTickets != null && pendingTickets.Any())
                                {
                                    foreach (var pendingTicket in pendingTickets)
                                    {
                                        if (pendingTicket.Status == RegisterAttendStatusEnums.Pending.ToString())
                                        {
                                            await _registerAttendRepo.DeleteRegisterAttend(pendingTicket.AttendId);

                                            var pendingOrder = await _orderRepo.GetOrderByServiceId(pendingTicket.GroupId);
                                            if (pendingOrder != null && pendingOrder.Status == PaymentStatusEnums.Pending.ToString())
                                            {
                                                pendingOrder.Status = PaymentStatusEnums.Canceled.ToString();
                                                pendingOrder.PaymentReference = null;
                                                await _orderRepo.UpdateOrder(pendingOrder);
                                            }
                                        }
                                    }
                                }
                            }
                            var allTickets = await _registerAttendRepo.GetRegisterAttends();
                            var pendingTicketsToGroup = allTickets.Where(x => x.Status == RegisterAttendStatusEnums.Pending.ToString() || x.Status == RegisterAttendStatusEnums.PendingConfirm.ToString());
                            var groupedTickets = pendingTicketsToGroup.GroupBy(x => x.GroupId).ToList();
                            foreach (var group in groupedTickets)
                            {
                                int totalTickets = group.Count();

                                if (totalTickets > workshop.Capacity)
                                {
                                    foreach (var ticket in group)
                                    {
                                        ticket.Status = RegisterAttendStatusEnums.Canceled.ToString();
                                        await _registerAttendRepo.UpdateRegisterAttend(ticket);
                                    }

                                    var pendingOrder = await _orderRepo.GetOrderByServiceId(group.Key);
                                    if (pendingOrder != null && pendingOrder.Status == PaymentStatusEnums.Pending.ToString())
                                    {
                                        pendingOrder.Status = PaymentStatusEnums.Canceled.ToString();
                                        pendingOrder.PaymentReference = null;
                                        await _orderRepo.UpdateOrder(pendingOrder);
                                    }
                                    if (pendingOrder != null && pendingOrder.Status == PaymentStatusEnums.PendingConfirm.ToString())
                                    {
                                        pendingOrder.Status = PaymentStatusEnums.WaitingForRefund.ToString();
                                        pendingOrder.PaymentReference = null;
                                        await _orderRepo.UpdateOrder(pendingOrder);
                                    }
                                }
                            }
                        }
                    }
                }

                // BookingOnline
                else if (order.ServiceType == PaymentTypeEnums.BookingOnline.ToString())
                {
                    var bookingOnline = await _bookingOnlineRepo.GetBookingOnlineByIdRepo(order.ServiceId);
                    if (string.IsNullOrEmpty(bookingOnline.MasterId))
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.ResponseCode = ResponseCodeConstants.FAILED;
                        res.Message = ResponseMessageConstrantsBooking.MISSINNG_MASTERID;
                        return res;
                    }

                    bookingOnline.Status = BookingOnlineEnums.Confirmed.ToString();
                    await _bookingOnlineRepo.UpdateBookingOnlineRepo(bookingOnline);

                    var masterSchedule = await _masterScheduleRepo.GetMasterScheduleById(bookingOnline.MasterScheduleId);
                    if (masterSchedule != null)
                    {
                        masterSchedule.Status = MasterScheduleEnums.InProgress.ToString();
                        await _masterScheduleRepo.UpdateMasterSchedule(masterSchedule);
                    }

                    var conflictingBookings = await _bookingOnlineRepo.GetBookingsByMasterAndTimeRepo(
                        bookingOnline.MasterId,
                        (TimeOnly)bookingOnline.StartTime,
                        (TimeOnly)bookingOnline.EndTime,
                        (DateOnly)bookingOnline.BookingDate);

                    foreach (var conflictBooking in conflictingBookings)
                    {
                        if (conflictBooking.BookingOnlineId == bookingOnline.BookingOnlineId)
                            continue;

                        conflictBooking.Status = BookingOnlineEnums.Canceled.ToString();
                        await _bookingOnlineRepo.UpdateBookingOnlineRepo(conflictBooking);

                        var conflictMasterSchedule = await _masterScheduleRepo.GetMasterScheduleById(conflictBooking.MasterScheduleId);
                        if (conflictMasterSchedule != null)
                        {
                            conflictMasterSchedule.Status = MasterScheduleEnums.Canceled.ToString();
                            await _masterScheduleRepo.UpdateMasterSchedule(conflictMasterSchedule);
                        }

                        var conflictOrder = await _orderRepo.GetOneOrderByService(conflictBooking.BookingOnlineId, PaymentTypeEnums.BookingOnline);
                        if (conflictOrder != null)
                        {
                            if (conflictOrder.Status == PaymentStatusEnums.Pending.ToString())
                            {
                                conflictOrder.PaymentReference = null;
                                conflictOrder.Status = PaymentStatusEnums.Canceled.ToString();
                            }
                            else if (conflictOrder.Status == PaymentStatusEnums.PendingConfirm.ToString())
                            {
                                conflictOrder.PaymentReference = null;
                                conflictOrder.Status = PaymentStatusEnums.WaitingForRefund.ToString();
                            }
                            await _orderRepo.UpdateOrder(conflictOrder);
                        }
                    }
                }

                // BookingOffline
                else if (order.ServiceType == PaymentTypeEnums.BookingOffline.ToString())
                {
                    bool isFirstPayment = order.Note != null && order.Note.Contains("Thanh toán đặt cọc 30%");
                    await UpdateBookingOfflineStatusAfterPayment(order.ServiceId, isFirstPayment);

                    if (isFirstPayment)
                    {
                        order.Status = PaymentStatusEnums.Paid1st.ToString();
                    }
                    else
                    {
                        order.Status = PaymentStatusEnums.Paid2nd.ToString();
                    }

                    await _orderRepo.UpdateOrder(order);
                }
                // Course
                else if (order.ServiceType == PaymentTypeEnums.Course.ToString())
                {
                    var Course = await _courseRepo.GetCourseById(order.ServiceId);
                    if (Course == null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.Message = ResponseMessageConstrantsCourse.COURSE_NOT_FOUND;
                        return res;
                    }

                    var registerCourse = new RegisterCourse
                    {
                        EnrollCourseId = GenerateShortGuid(),
                        CourseId = Course.CourseId,
                        EnrollCertId = null,
                        EnrollQuizId = null,
                        Percentage = 0,
                        Status = EnrollChapterStatusEnums.InProgress.ToString(),
                        CustomerId = order.CustomerId
                    };

                    var result = await _registerCourseRepo.CreateRegisterCourse(registerCourse);

                    var chapters = await _chapterRepo.GetChaptersByCourseId(Course.CourseId);
                    if (chapters != null && chapters.Any())
                    {
                        foreach (var chapter in chapters)
                        {
                            var enrollChapter = new EnrollChapter
                            {
                                EnrollChapterId = GenerateShortGuid(),
                                ChapterId = chapter.ChapterId,
                                Status = EnrollChapterStatusEnums.InProgress.ToString(),
                                EnrollCourseId = result.EnrollCourseId
                            };

                            await _enrollChapterRepo.CreateEnrollChapter(enrollChapter);
                        }
                    }

                    if (order.Status.Equals(PaymentStatusEnums.Paid))
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status500InternalServerError;
                        res.ResponseCode = ResponseCodeConstants.FAILED;
                        res.Message = ResponseMessageConstrantsOrder.ALREADY_PAID;
                        return res;
                    }

                    if (result == null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status500InternalServerError;
                        res.ResponseCode = ResponseCodeConstants.FAILED;
                        res.Message = ResponseMessageConstrantsCourse.COURSE_CREATED_FAILED;
                        return res;
                    }
                }

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
                res.Message = ex.InnerException?.Message;
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

        public async Task<ResultModel> GetPendingConfirmOrders()
        {
            var res = new ResultModel();
            try
            {
                var orders = await _orderRepo.GetAllOrders();
                var pendingConfirmOrders = orders.Where(x => x.Status == PaymentStatusEnums.PendingConfirm.ToString()).OrderBy(x => x.PaymentDate).ToList();

                if (pendingConfirmOrders == null || !pendingConfirmOrders.Any() || pendingConfirmOrders.Count == 0)
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
                res.Data = _mapper.Map<List<OrderResponse>>(pendingConfirmOrders);
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

        public async Task<ResultModel> GetWaitingForRefundOrders()
        {
            var res = new ResultModel();
            try
            {
                var orders = await _orderRepo.GetAllOrders();
                var pendingConfirmOrders = orders.Where(x => x.Status == PaymentStatusEnums.WaitingForRefund.ToString()).OrderByDescending(x => x.CreatedDate).ToList();

                if (pendingConfirmOrders == null || !pendingConfirmOrders.Any() || pendingConfirmOrders.Count == 0)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsOrder.NOT_FOUND_WAITINGFORREFUND;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Data = _mapper.Map<List<OrderResponse>>(pendingConfirmOrders);
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

        public async Task<ResultModel> GetDetailsOrder(string id)
        {
            var res = new ResultModel();
            try
            {
                var orders = await _orderRepo.GetOrderById(id);
                if (orders == null)
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
                res.Data = _mapper.Map<OrderResponse>(orders);
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

        public async Task<ResultModel> CancelOrder(string serviceId, PaymentTypeEnums serviceType)
        {
            var res = new ResultModel();
            try
            {
                var order = await _orderRepo.GetOneOrderByService(serviceId, serviceType);
                
                switch (serviceType)
                {
                    case PaymentTypeEnums.BookingOnline:
                        var bookingOnline = await _bookingOnlineRepo.GetBookingOnlineByIdRepo(serviceId);
                        if (bookingOnline == null)
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                            res.StatusCode = StatusCodes.Status404NotFound;
                            res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_ONLINE;
                            return res;
                        }

                        bookingOnline.Status = BookingOnlineEnums.Canceled.ToString();
                        await _bookingOnlineRepo.UpdateBookingOnlineRepo(bookingOnline);

                        if (!string.IsNullOrEmpty(bookingOnline.MasterScheduleId))
                        {
                            var masterSchedule = await _masterScheduleRepo.GetMasterScheduleById(bookingOnline.MasterScheduleId);
                            if (masterSchedule != null)
                            {
                                masterSchedule.Status = MasterScheduleEnums.Canceled.ToString();
                                await _masterScheduleRepo.UpdateMasterSchedule(masterSchedule);
                            }
                        }

                        if (order != null)
                        {
                            if (order.Status == PaymentStatusEnums.Pending.ToString())
                            {
                                order.Status = PaymentStatusEnums.Canceled.ToString();
                                order.PaymentReference = null;
                            }
                            if (order.Status == PaymentStatusEnums.PendingConfirm.ToString())
                            {
                                order.Status = PaymentStatusEnums.WaitingForRefund.ToString();
                                order.PaymentReference = null;
                            }
                        }
                        break;

                    case PaymentTypeEnums.BookingOffline:
                        var bookingOffline = await _bookingOfflineRepo.GetBookingOfflineById(serviceId);
                        if (bookingOffline == null)
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                            res.StatusCode = StatusCodes.Status404NotFound;
                            res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                            return res;
                        }

                        bookingOffline.Status = BookingOfflineEnums.Canceled.ToString();
                        await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);
                        break;

                    case PaymentTypeEnums.Course:
                        if (order != null && order.Status == PaymentStatusEnums.Paid.ToString())
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.FAILED;
                            res.StatusCode = StatusCodes.Status400BadRequest;
                            res.Message = "Bạn không thể hủy khóa học bạn đã thanh toán";
                            return res;
                        }

                        if (order != null)
                        {
                            order.Status = PaymentStatusEnums.Canceled.ToString();
                            order.PaymentReference = null;
                        }
                        break;

                    case PaymentTypeEnums.RegisterAttend:
                        var tickets = await _registerAttendRepo.GetRegisterAttendsByGroupId(serviceId);
                        if (tickets == null || !tickets.Any())
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                            res.StatusCode = StatusCodes.Status404NotFound;
                            res.Message = "Không tìm thấy thông tin đăng ký tham dự";
                            return res;
                        }

                        foreach (var ticket in tickets)
                        {
                            ticket.Status = RegisterAttendStatusEnums.Canceled.ToString();
                            await _registerAttendRepo.UpdateRegisterAttend(ticket);
                        }

                        if (order != null)
                        {
                            if (order.Status == PaymentStatusEnums.Pending.ToString())
                            {
                                order.Status = PaymentStatusEnums.Canceled.ToString();
                                order.PaymentReference = null;
                            }
                            if (order.Status == PaymentStatusEnums.PendingConfirm.ToString())
                            {
                                order.Status = PaymentStatusEnums.WaitingForRefund.ToString();
                                order.PaymentReference = null;
                            }
                        }
                        break;

                    default:
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.FAILED;
                        res.StatusCode = StatusCodes.Status400BadRequest;
                        res.Message = ResponseMessageConstrantsOrder.SERVICETYPE_INVALID;
                        return res;
                }

                if (order != null)
                {
                    await _orderRepo.UpdateOrder(order);
                }

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
                    switch (order.ServiceType)
                    {
                        case nameof(PaymentTypeEnums.BookingOnline):
                            var bookingOnline = await _bookingOnlineRepo.GetBookingOnlineByIdRepo(order.ServiceId);
                            if (bookingOnline != null)
                            {
                                bookingOnline.Status = BookingOnlineEnums.Canceled.ToString();
                                await _bookingOnlineRepo.UpdateBookingOnlineRepo(bookingOnline);

                                if (!string.IsNullOrEmpty(bookingOnline.MasterScheduleId))
                                {
                                    var masterSchedule = await _masterScheduleRepo.GetMasterScheduleById(bookingOnline.MasterScheduleId);
                                    if (masterSchedule != null)
                                    {
                                        masterSchedule.Status = MasterScheduleEnums.Canceled.ToString();
                                        await _masterScheduleRepo.UpdateMasterSchedule(masterSchedule);
                                    }
                                }
                            }
                            break;

                        case nameof(PaymentTypeEnums.RegisterAttend):
                            var tickets = await _registerAttendRepo.GetRegisterAttendsByGroupId(order.ServiceId);
                            if (tickets != null && tickets.Any())
                            {
                                foreach (var ticket in tickets)
                                {
                                    ticket.Status = RegisterAttendStatusEnums.Canceled.ToString();
                                    await _registerAttendRepo.UpdateRegisterAttend(ticket);
                                }
                            }
                            break;
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
