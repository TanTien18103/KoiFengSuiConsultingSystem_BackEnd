using AutoMapper;
using Microsoft.AspNetCore.Http;
using Services.ApiModels;
using Services.ApiModels.BookingOnline;
using BusinessObjects.Enums;
using Services.ApiModels.BookingOffline;
using Services.ApiModels.Booking;
using BusinessObjects.Models;
using Repositories.Repositories;
using Services.Services.MasterScheduleService;
using Repositories.Repositories.AccountRepository;
using Repositories.Repositories.BookingOfflineRepository;
using Repositories.Repositories.BookingOnlineRepository;
using Repositories.Repositories.CustomerRepository;
using BusinessObjects.Constants;
using System.Security.Claims;
using Repositories.Repositories.MasterRepository;
using Repositories.Repositories.MasterScheduleRepository;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;
using System.Security.Cryptography.Xml;
using System.Diagnostics;
using Repositories.Repositories.ConsultationPackageRepository;
using System.Runtime.CompilerServices;
using System.Linq;
using Repositories.Repositories.OrderRepository;
using Services.ApiModels.KoiVariety;

namespace Services.Services.BookingService
{
    public class BookingService : IBookingService
    {
        private readonly IBookingOnlineRepo _onlineRepo;
        private readonly IBookingOfflineRepo _offlineRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerRepo _customerRepo;
        private readonly IAccountRepo _accountRepo;
        private readonly IMasterScheduleRepo _masterScheduleRepo;
        private readonly IConsultationPackageRepo _consultationPackageRepo;
        private readonly IMasterRepo _masterRepo;
        private readonly IOrderRepo _orderRepo;

        public BookingService(
            IBookingOnlineRepo onlineRepo,
            IBookingOfflineRepo offlineRepo,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ICustomerRepo customerRepo,
            IAccountRepo accountRepo,
            IMasterScheduleRepo masterScheduleRepo,
            IConsultationPackageRepo consultationPackageRepo,
            IMasterRepo masterRepo,
            IOrderRepo orderRepo
        )
        {
            _onlineRepo = onlineRepo;
            _offlineRepo = offlineRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _customerRepo = customerRepo;
            _accountRepo = accountRepo;
            _masterScheduleRepo = masterScheduleRepo;
            _consultationPackageRepo = consultationPackageRepo;
            _masterRepo = masterRepo;
            _orderRepo = orderRepo;
        }

        private string GetAuthenticatedAccountId()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated) return null;

            return identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        //Booking Online
        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }

        public async Task<ResultModel> CreateBookingOnline(BookingOnlineRequest request)
        {
            var res = new ResultModel();
            try
            {
                if (request == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsBooking.REQUIRED_DATA;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var token = authHeader.Substring("Bearer ".Length);
                var accountId = await _accountRepo.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var customer = await _customerRepo.GetCustomerByAccountId(accountId);
                if (string.IsNullOrEmpty(customer.CustomerId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.CUSTOMER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                var currentTime = TimeOnly.FromDateTime(DateTime.Now);

                if (request.BookingDate < currentDate ||
                    (request.BookingDate == currentDate && request.StartTime <= currentTime))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsBooking.TIME_PASSED;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var hasUncompletedBooking = await _onlineRepo.CheckCustomerHasUncompletedBookingRepo(customer.CustomerId);
                if (hasUncompletedBooking.HasPendingBooking)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsBooking.ALREADY_CREATE_BOOKING;
                    res.StatusCode = StatusCodes.Status402PaymentRequired;
                    return res;
                }

                if (hasUncompletedBooking.HasPendingConfirmBooking)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsBooking.WAITING_CONFIRM;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                if (!string.IsNullOrEmpty(request.MasterId))
                {
                    var existingBookings = await _onlineRepo.GetBookingsByMasterAndTimeRepo(
                        request.MasterId,
                        request.StartTime,
                        request.EndTime,
                        request.BookingDate);

                    foreach (var existingBooking in existingBookings)
                    {
                        var existingOrder = await _orderRepo.GetOneOrderByService(
                            existingBooking.BookingOnlineId,
                            PaymentTypeEnums.BookingOnline);

                        if (existingOrder != null &&
                            (existingOrder.Status == PaymentStatusEnums.Paid.ToString() ||
                             existingOrder.Status == PaymentStatusEnums.PendingConfirm.ToString()))
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.FAILED;
                            res.Message = ResponseMessageConstrantsMaster.EXISTING_SCHEDULE;
                            res.StatusCode = StatusCodes.Status400BadRequest;
                            return res;
                        }
                    }
                }

                string masterScheduleId = null;

                var masterSchedule = new MasterSchedule
                {
                    MasterScheduleId = GenerateShortGuid(),
                    MasterId = request.MasterId,
                    Date = request.BookingDate,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Type = BookingTypeEnums.Online.ToString(),
                    Status = BookingOnlineEnums.Pending.ToString(),
                };

                await _masterScheduleRepo.CreateMasterSchedule(masterSchedule);
                masterScheduleId = masterSchedule.MasterScheduleId;

                var booking = _mapper.Map<BookingOnline>(request);
                booking.BookingOnlineId = GenerateShortGuid();
                booking.CustomerId = customer.CustomerId;
                booking.Status = BookingOnlineEnums.Pending.ToString();
                booking.MasterScheduleId = masterScheduleId;
                booking.Price = 3000;

                var createdBooking = await _onlineRepo.CreateBookingOnlineRepo(booking);
                var bookingResponse = await _onlineRepo.GetBookingOnlineByIdRepo(createdBooking.BookingOnlineId);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Message = ResponseMessageConstrantsBooking.BOOKING_CREATED;
                res.Data = _mapper.Map<BookingOnlineDetailResponse>(bookingResponse);

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi tạo booking: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetBookingByIdAsync(string bookingId)
        {
            var res = new ResultModel();
            try
            {
                var onlineBooking = await _onlineRepo.GetBookingOnlineByIdRepo(bookingId);
                if (onlineBooking != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<BookingOnlineDetailResponse>(onlineBooking);
                    res.Message = ResponseMessageConstrantsBooking.ONLINE_GET_SUCCESS;
                    return res;
                }

                var offlineBooking = await _offlineRepo.GetBookingOfflineById(bookingId);
                if (offlineBooking != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<BookingOfflineDetailResponse>(offlineBooking);
                    res.Message = ResponseMessageConstrantsBooking.OFFLINE_GET_SUCCESS;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy thông tin booking: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetConsultingDetailByMasterScheduleIdAsync(string masterScheduleId)
        {
            var res = new ResultModel();
            try
            {
                var onlineBooking = await _onlineRepo.GetConsultingOnlineByMasterScheduleIdRepo(masterScheduleId);
                if (onlineBooking != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<ConsultingOnlineDetailResponse>(onlineBooking);
                    res.Message = ResponseMessageConstrantsBooking.ONLINE_GET_SUCCESS;
                    return res;
                }

                var offlineBooking = await _offlineRepo.GetConsultingOfflineByMasterScheduleIdRepo(masterScheduleId);
                if (offlineBooking != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<ConsultingOfflineDetailResponse>(offlineBooking);
                    res.Message = ResponseMessageConstrantsBooking.OFFLINE_GET_SUCCESS;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy thông tin consulting: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetBookingByStatusAsync(BookingOnlineEnums? status = null, BookingTypeEnums? type = null)
        {
            var res = new ResultModel();
            try
            {
                List<BookingResponse> bookingList = new List<BookingResponse>();

                if (type == null || type == BookingTypeEnums.Online)
                {
                    var bookingOnlineList = await _onlineRepo.GetBookingOnlinesRepo();
                    var filteredBookings = bookingOnlineList;

                    if (status.HasValue)
                    {
                        filteredBookings = filteredBookings.Where(x => x.Status == status.ToString()).ToList();
                    }

                    if (filteredBookings.Any())
                    {
                        var mappedOnlineBookings = _mapper.Map<List<BookingResponse>>(filteredBookings);
                        // Ensure Type is set to Online for all online bookings
                        foreach (var booking in mappedOnlineBookings)
                        {
                            booking.Type = BookingTypeEnums.Online.ToString();
                        }
                        bookingList.AddRange(mappedOnlineBookings);
                    }
                    else if (status.HasValue && type == BookingTypeEnums.Online)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_STATUS + status;
                        return res;
                    }
                }

                if (type == null || type == BookingTypeEnums.Offline)
                {
                    var bookingOfflineList = await _offlineRepo.GetBookingOfflines();
                    var filteredBookings = bookingOfflineList;

                    if (status.HasValue)
                    {
                        filteredBookings = filteredBookings.Where(x => x.Status == status.ToString()).ToList();
                    }

                    if (filteredBookings.Any())
                    {
                        var mappedOfflineBookings = _mapper.Map<List<BookingResponse>>(filteredBookings);
                        // Ensure Type is set to Offline for all offline bookings
                        foreach (var booking in mappedOfflineBookings)
                        {
                            booking.Type = BookingTypeEnums.Offline.ToString();
                        }
                        bookingList.AddRange(mappedOfflineBookings);
                    }
                    else if (status.HasValue && type == BookingTypeEnums.Offline)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_STATUS + status;
                        return res;
                    }
                }

                if (!bookingList.Any())
                {
                    string typeMessage = type.HasValue ? type == BookingTypeEnums.Online ? "online" : "offline" : "online và offline";
                    string statusMessage = status.HasValue ? $"với trạng thái {status}" : "";

                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND + " " + typeMessage + " " + statusMessage;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = bookingList;

                string typeSuccessMessage = type.HasValue ? type == BookingTypeEnums.Online ? "online" : "offline" : "online và offline";
                string statusSuccessMessage = status.HasValue ? $"với trạng thái {status}" : "";

                res.Message = $"Lấy danh sách buổi tư vấn {typeSuccessMessage} {statusSuccessMessage} thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetBookingOnlinesHoverAsync()
        {
            var res = new ResultModel();
            try
            {
                var bookings = await _onlineRepo.GetBookingOnlinesRepo();
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<BookingOnlineHoverRespone>>(bookings);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Lỗi khi lấy danh sách booking: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> AssignMasterToBookingAsync(string? bookingonlineId, string? bookingofflineId, string masterId)
        {
            var res = new ResultModel();
            try
            {
                if (string.IsNullOrEmpty(bookingonlineId) && string.IsNullOrEmpty(bookingofflineId) && string.IsNullOrEmpty(masterId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.REQUIRED_DATA;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                var bookingOnline = await _onlineRepo.GetBookingOnlineByIdRepo(bookingonlineId);
                var bookingOffline = await _offlineRepo.GetBookingOfflineById(bookingofflineId);

                var masterschedules = await _masterScheduleRepo.GetMasterScheduleByMasterId(masterId);
                if (masterschedules == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsMasterSchedule.MASTERSCHEDULE_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                if (string.IsNullOrEmpty(bookingofflineId) && string.IsNullOrEmpty(bookingonlineId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.REQUIRED_ONE_ATLEAST;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                if (!string.IsNullOrEmpty(bookingofflineId) && !string.IsNullOrEmpty(bookingonlineId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.REQUIRED_ONE;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                if (string.IsNullOrEmpty(bookingofflineId) && !string.IsNullOrEmpty(bookingonlineId))
                {

                    if (bookingOnline == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        return res;
                    }

                    if (!string.IsNullOrEmpty(bookingOnline.MasterId))
                    {

                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.EXISTED;
                        res.Message = ResponseMessageConstrantsBooking.ALREADY_ASSIGNED;
                        res.StatusCode = StatusCodes.Status400BadRequest;
                        return res;
                    }

                    foreach (var masterschedule in masterschedules)
                    {
                        if (masterschedule.Date == bookingOnline.BookingDate && masterschedule.StartTime == bookingOnline.StartTime && masterschedule.EndTime == bookingOnline.EndTime)
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.EXISTED;
                            res.Message = ResponseMessageConstrantsMasterSchedule.MASTERSCHEDULE_EXISTED_SLOT;
                            res.StatusCode = StatusCodes.Status409Conflict;
                            return res;
                        }
                    }

                    var masterSchedule = new MasterSchedule
                    {
                        MasterScheduleId = GenerateShortGuid(),
                        MasterId = masterId,
                        Date = bookingOnline.BookingDate,
                        StartTime = bookingOnline.StartTime,
                        EndTime = bookingOnline.EndTime,
                        Type = MasterScheduleTypeEnums.BookingOnline.ToString(),
                        Status = MasterScheduleEnums.Pending.ToString(),
                    };
                    var createmasterSchedule = await _masterScheduleRepo.CreateMasterSchedule(masterSchedule);

                    bookingOnline.MasterId = masterId;
                    await _onlineRepo.UpdateBookingOnlineRepo(bookingOnline);
                }
                if (!string.IsNullOrEmpty(bookingofflineId) && string.IsNullOrEmpty(bookingonlineId))
                {
                    if (bookingOffline == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        return res;
                    }

                    if (!string.IsNullOrEmpty(bookingOffline.MasterId))
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.EXISTED;
                        res.Message = ResponseMessageConstrantsBooking.ALREADY_ASSIGNED;
                        res.StatusCode = StatusCodes.Status400BadRequest;
                        return res;
                    }

                    foreach (var masterschedule in masterschedules)
                    {
                        if (masterschedule.Date == bookingOffline.StartDate)
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.EXISTED;
                            res.Message = ResponseMessageConstrantsMasterSchedule.MASTERSCHEDULE_EXISTED_SLOT;
                            res.StatusCode = StatusCodes.Status409Conflict;
                            return res;
                        }
                    }

                    var masterSchedule = new MasterSchedule
                    {
                        MasterScheduleId = GenerateShortGuid(),
                        MasterId = masterId,
                        Date = bookingOffline.StartDate,
                        Type = MasterScheduleTypeEnums.BookingOffline.ToString(),
                        Status = MasterScheduleEnums.Pending.ToString(),
                    };
                    
                    var createmasterSchedule = await _masterScheduleRepo.CreateMasterSchedule(masterSchedule);

                    bookingOffline.MasterId = masterId;
                    await _offlineRepo.UpdateBookingOffline(bookingOffline);
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsBooking.ASSIGNED_SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi gán Master: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> AssignStaffToBookingAsync(string? bookingonlineId, string? bookingofflineId, string staffId)
        {
            var res = new ResultModel();
            try
            {
                var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var claims = identity.Claims;
                var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                if (string.IsNullOrEmpty(bookingonlineId) && string.IsNullOrEmpty(bookingofflineId) && string.IsNullOrEmpty(staffId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.REQUIRED_DATA;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                var bookingOnline = await _onlineRepo.GetBookingOnlineByIdRepo(bookingonlineId);
                var bookingOffline = await _offlineRepo.GetBookingOfflineById(bookingofflineId);

                if (string.IsNullOrEmpty(bookingofflineId) && string.IsNullOrEmpty(bookingonlineId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.REQUIRED_ONE_ATLEAST;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                if (!string.IsNullOrEmpty(bookingofflineId) && !string.IsNullOrEmpty(bookingonlineId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.REQUIRED_ONE;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                if (string.IsNullOrEmpty(bookingofflineId) && !string.IsNullOrEmpty(bookingonlineId))
                {

                    if (bookingOnline == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        return res;
                    }

                    if (!string.IsNullOrEmpty(bookingOnline.MasterId))
                    {

                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.EXISTED;
                        res.Message = ResponseMessageConstrantsBooking.ALREADY_ASSIGNED;
                        res.StatusCode = StatusCodes.Status400BadRequest;
                        return res;
                    }

                    bookingOnline.AssignStaffId = staffId;
                    await _onlineRepo.UpdateBookingOnlineRepo(bookingOnline);
                }
                if (!string.IsNullOrEmpty(bookingofflineId) && string.IsNullOrEmpty(bookingonlineId))
                {
                    if (bookingOffline == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        return res;
                    }

                    if (!string.IsNullOrEmpty(bookingOffline.MasterId))
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.EXISTED;
                        res.Message = ResponseMessageConstrantsBooking.ALREADY_ASSIGNED;
                        res.StatusCode = StatusCodes.Status400BadRequest;
                        return res;
                    }

                    bookingOffline.AssignStaffId = staffId;
                    await _offlineRepo.UpdateBookingOffline(bookingOffline);
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsBooking.ASSIGNED_SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi gán Master: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetBookingOnlineById(string id)
        {
            var res = new ResultModel();
            try
            {
                var onlineBooking = await _onlineRepo.GetBookingOnlineByIdRepo(id);
                if (onlineBooking != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<BookingOnlineDetailResponse>(onlineBooking);
                    res.Message = ResponseMessageConstrantsBooking.ONLINE_GET_SUCCESS;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy thông tin booking: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> UpdateCompletedBookingOnline(string id)
        {
            var res = new ResultModel();
            try
            {
                var onlineBooking = await _onlineRepo.GetBookingOnlineByIdRepo(id);
                if (onlineBooking == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                    return res;
                }

                if (onlineBooking.Status == BookingOnlineEnums.Completed.ToString())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = "Buổi tư vấn đã hoàn thành";
                    return res;
                }
                if (onlineBooking.Status != BookingOnlineEnums.Confirmed.ToString())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = "Buổi tư vấn vẫn còn trong quá trình thanh toán và xác nhận";
                    return res;
                }

                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var now = TimeOnly.FromDateTime(DateTime.Now);

                if (onlineBooking.BookingDate.HasValue && onlineBooking.StartTime.HasValue)
                {
                    if (onlineBooking.BookingDate.Value > today || (onlineBooking.BookingDate.Value == today && onlineBooking.StartTime.Value > now))
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                        res.StatusCode = StatusCodes.Status400BadRequest;
                        res.Message = "Buổi tư vấn chưa kết thúc, không thể cập nhật trạng thái.";
                        return res;
                    }
                }

                onlineBooking.Status = BookingOnlineEnums.Completed.ToString();
                await _onlineRepo.UpdateBookingOnlineRepo(onlineBooking);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = "Cập nhật trạng thái thành công.";
                return res;

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> Calculate(BookingTypeEnums bookingType, string bookingId)
        {
            var res = new ResultModel();
            switch (bookingType)
            {
                case BookingTypeEnums.Online:
                    var onlineBooking = await _onlineRepo.GetBookingOnlineByIdRepo(bookingId);
                    if (onlineBooking == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = ResponseMessageConstrantsBooking.NOT_FOUND + " online";
                        return res;
                    }
                    // Type is already set by the mapper profile
                    res.IsSuccess = true;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.Data = _mapper.Map<BookingResponse>(onlineBooking); ;
                    res.Message = ResponseMessageConstrantsBooking.ONLINE_GET_SUCCESS;
                    return res;

                case BookingTypeEnums.Offline:
                    var offlineBooking = await _offlineRepo.GetBookingOfflineById(bookingId);
                    if (offlineBooking == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = ResponseMessageConstrantsBooking.NOT_FOUND + " offline";
                        return res;
                    }
                    // Type is already set by the mapper profile
                    res.IsSuccess = true;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.Data = _mapper.Map<BookingResponse>(offlineBooking); ;
                    res.Message = ResponseMessageConstrantsBooking.OFFLINE_GET_SUCCESS;
                    return res;

                default:
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsBooking.INVALID_DATA;
                    return res;
            }
        }
        public async Task<ResultModel> CompleteBookingOnlineByMaster(string bookingOnlineId)
        {
            var res = new ResultModel();
            try
            {
                // Chỉ lấy BookingOnline mà không tải các entity liên quan
                var bookingOnline = await _onlineRepo.GetBookingOnlineByIdRepo(bookingOnlineId);
                if (bookingOnline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_ONLINE;
                    return res;
                }


                var updateSuccess = await _onlineRepo.UpdateBookingOnlineStatusRepo(bookingOnlineId, BookingOnlineEnums.Completed.ToString());

                if (updateSuccess == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = ResponseMessageConstrantsBooking.UPDATE_STATUS_BOOKING_ONL_FAILED;
                    return res;
                }


                var updatedBooking = await _onlineRepo.GetBookingOnlineByIdRepo(bookingOnlineId);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsBooking.COMPLETE_BOOKING_SUCCESS;
                res.Data = _mapper.Map<BookingResponse>(updatedBooking);

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi cập nhật trạng thái buổi tư vấn: {ex.Message}";
                return res;
            }
        }
        public async Task<ResultModel> UpdateBookingOnlineMasterNote(string bookingOnlineId, UpdateMasterNoteRequest request)
        {
            var res = new ResultModel();
            try
            {

                var bookingOnline = await _onlineRepo.GetBookingOnlineByIdRepo(bookingOnlineId);
                if (bookingOnline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_ONLINE;
                    return res;
                }


                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }


                var master = await _masterRepo.GetMasterByAccountId(accountId);
                if (master == null || bookingOnline.MasterId != master.MasterId)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }


                var updatedBooking = await _onlineRepo.UpdateBookingOnlineMasterNoteRepo(bookingOnlineId, request.MasterNote);
                if (updatedBooking == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = ResponseMessageConstrantsBooking.UPDATE_MASTER_NOTE_BOOKING_ONL_FAILED;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsBooking.UPDATE_MASTER_NOTE_BOOKING_ONL_SUCCESS;
                res.Data = _mapper.Map<BookingResponse>(updatedBooking);

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi cập nhật ghi chú: {ex.Message}";
                return res;
            }
        }
        public async Task<ResultModel> GetBookingOnlinesByMaster()
        {
            var res = new ResultModel();
            try
            {

                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }


                var master = await _masterRepo.GetMasterByAccountId(accountId);
                if (master == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                    res.Message = "Bạn không phải là Master";
                    return res;
                }


                var bookings = await _onlineRepo.GetBookingOnlinesByMasterIdRepo(master.MasterId);

                if (bookings == null || !bookings.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_ONLINE;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsBooking.GET_ALL_BOOKING_ONLINE_SUCCESS;
                res.Data = _mapper.Map<List<BookingOnlineDetailResponse>>(bookings);

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy danh sách buổi tư vấn: {ex.Message}";
                return res;
            }
        }
        // Bookin Offline 
        public async Task<ResultModel> GetBookingOfflineById(string id)
        {
            var res = new ResultModel();
            try
            {
                var offlineBooking = await _offlineRepo.GetBookingOfflineById(id);
                if (offlineBooking != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<BookingOfflineDetailResponse>(offlineBooking);
                    res.Message = ResponseMessageConstrantsBooking.OFFLINE_GET_SUCCESS;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy thông tin booking: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> RemoveConsultationPackage(string id)
        {
            var res = new ResultModel();
            try
            {
                var bookingOffline = await _offlineRepo.GetBookingOfflineById(id);
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }

                if (bookingOffline.ConsultationPackageId == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsPackage.PACKAGE_NOT_FOUND;
                    return res;
                }
                bookingOffline.ConsultationPackageId = null;
                await _offlineRepo.UpdateBookingOffline(bookingOffline);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsPackage.REMOVED_PACKAGE;
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

        public async Task<ResultModel> ProcessCompleteBooking(BookingOfflineRequest request, string packageId, decimal selectedPrice)
        {
            var res = new ResultModel();
            try
            {
                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }

                var customerId = await _customerRepo.GetCustomerIdByAccountId(accountId);
                var consultationPackage = await _consultationPackageRepo.GetConsultationPackageById(packageId);
                if (consultationPackage == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsPackage.PACKAGE_NOT_FOUND;
                    return res;
                }

                if (selectedPrice != consultationPackage.MinPrice &&
                    selectedPrice != consultationPackage.MaxPrice)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsBooking.PRICE_SELECTED_INVALID;
                    return res;
                }

                var bookingOffline = _mapper.Map<BookingOffline>(request);
                bookingOffline.BookingOfflineId = GenerateShortGuid();
                bookingOffline.CustomerId = customerId;
                bookingOffline.Status = BookingOfflineEnums.Pending.ToString();

                var (completedBooking, message) = await _offlineRepo.ProcessBookingTransaction(
                    bookingOffline, packageId, selectedPrice);

                if (completedBooking == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = message;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status201Created;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsBooking.BOOKING_CREATED;
                res.Data = _mapper.Map<BookingOfflineDetailResponse>(completedBooking);
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

        public async Task<ResultModel> GetBookingOfflineForCurrentLogin()
        {
            var res = new ResultModel();
            try
            {
                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }
                var bookingOffline = await _offlineRepo.GetBookingOfflinesByAccountId(accountId);
                if (bookingOffline == null || !bookingOffline.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsBooking.OFFLINE_GET_SUCCESS;
                res.Data = bookingOffline.Select(b => _mapper.Map<BookingOfflineContractResponse>(b)).ToList();
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

        public async Task<ResultModel> GetBookingByTypeAndStatus(BookingTypeEnums? type, BookingOnlineEnums? onlineStatus, BookingOfflineEnums? offlineStatus)
        {
            var res = new ResultModel();
            try
            {

                var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var claims = identity.Claims;
                var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }
                var customerId = await _customerRepo.GetCustomerIdByAccountId(accountId);

                var onlineBookings = await _onlineRepo.GetBookingsOnlineByCustomerId(customerId) ?? new List<BookingOnline>();
                var offlineBookings = await _offlineRepo.GetBookingsOfflineByCustomerId(customerId) ?? new List<BookingOffline>();

                // Nếu cả 3 tham số đều null, lấy toàn bộ danh sách online & offline
                if (!type.HasValue && !onlineStatus.HasValue && !offlineStatus.HasValue)
                {
                    if (!onlineBookings.Any() && !offlineBookings.Any())
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.NOT_FOUND,
                            StatusCode = StatusCodes.Status404NotFound,
                            Message = ResponseMessageConstrantsBooking.NOT_FOUND
                        };
                    }

                    var allBookings = onlineBookings.Cast<object>().Concat(offlineBookings.Cast<object>()).ToList();

                    return new ResultModel
                    {
                        IsSuccess = true,
                        StatusCode = StatusCodes.Status200OK,
                        ResponseCode = ResponseCodeConstants.SUCCESS,
                        Data = _mapper.Map<List<BookingResponse>>(allBookings),
                        Message = ResponseMessageConstrantsBooking.BOOKING_FOUND
                    };
                }

                List<object> filteredBookings = new();

                switch (type)
                {
                    case BookingTypeEnums.Online:
                        if (!onlineBookings.Any())
                        {
                            return new ResultModel
                            {
                                IsSuccess = false,
                                ResponseCode = ResponseCodeConstants.NOT_FOUND,
                                StatusCode = StatusCodes.Status404NotFound,
                                Message = ResponseMessageConstrantsBooking.NOT_FOUND + " online"
                            };
                        }

                        if (onlineStatus.HasValue)
                        {
                            onlineBookings = onlineBookings.Where(x => x.Status == onlineStatus.Value.ToString()).ToList();
                        }

                        filteredBookings = onlineBookings.Cast<object>().ToList();
                        break;

                    case BookingTypeEnums.Offline:
                        if (!offlineBookings.Any())
                        {
                            return new ResultModel
                            {
                                IsSuccess = false,
                                ResponseCode = ResponseCodeConstants.NOT_FOUND,
                                StatusCode = StatusCodes.Status404NotFound,
                                Message = ResponseMessageConstrantsBooking.NOT_FOUND + " offline"
                            };
                        }

                        if (offlineStatus.HasValue)
                        {
                            offlineBookings = offlineBookings.Where(x => x.Status == offlineStatus.Value.ToString()).ToList();
                        }

                        filteredBookings = offlineBookings.Cast<object>().ToList();
                        break;

                    default:
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.INVALID_INPUT,
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = "Invalid booking type"
                        };
                }

                return new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Data = _mapper.Map<List<BookingResponse>>(filteredBookings),
                    Message = type == BookingTypeEnums.Online ? ResponseMessageConstrantsBooking.ONLINE_GET_SUCCESS : ResponseMessageConstrantsBooking.OFFLINE_GET_SUCCESS
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetBookingOfflinesByMaster()
        {
            var res = new ResultModel();
            try
            {
                // Lấy thông tin người dùng hiện tại
                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }

                // Lấy thông tin master từ accountId
                var master = await _masterRepo.GetMasterByAccountId(accountId);
                if (master == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                    res.Message = "Bạn không phải là Master";
                    return res;
                }

                // Lấy danh sách booking offline của master
                var bookings = await _offlineRepo.GetBookingOfflinesByMasterIdRepo(master.MasterId);

                if (bookings == null || !bookings.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsBooking.GET_ALL_BOOKING_ONLINE_SUCCESS;
                res.Data = _mapper.Map<List<BookingOfflineDetailResponse>>(bookings);

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy danh sách buổi tư vấn offline: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> CancelUnpaidBookings()
        {
            var res = new ResultModel();
            try
            {
                // Lấy thông tin người dùng hiện tại
                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }

                // Lấy thông tin master từ accountId
                var master = await _masterRepo.GetMasterByAccountId(accountId);
                if (master == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                    res.Message = "Bạn không phải là Master";
                    return res;
                }

                // Lấy danh sách booking offline của master
                var bookings = await _offlineRepo.GetBookingOfflinesByMasterIdRepo(master.MasterId);

                if (bookings == null || !bookings.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }
                // Lấy thời điểm 24 giờ trước
                var cutoffDate = DateTime.Now.AddDays(-1);

                // Lấy các booking chưa thanh toán tạo trước thời điểm cutoff
                var unpaidBookings = await _onlineRepo.GetUnpaidBookingsOlderThanRepo(cutoffDate);

                int cancelledCount = 0;
                foreach (var booking in unpaidBookings)
                {
                    // Tìm đơn hàng cho booking này
                    var order = await _orderRepo.GetOneOrderByService(
                        booking.BookingOnlineId,
                        PaymentTypeEnums.BookingOnline);

                    // Nếu không có đơn hàng hoặc đơn hàng chưa thanh toán, hủy booking
                    if (order == null || order.Status != PaymentStatusEnums.Paid.ToString())
                    {
                        await _onlineRepo.UpdateBookingOnlineStatusRepo(
                            booking.BookingOnlineId, 
                            BookingOnlineEnums.Canceled.ToString());
                        cancelledCount++;
                    }
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsBooking.GET_ALL_BOOKING_ONLINE_SUCCESS;
                res.Data = _mapper.Map<List<BookingOfflineDetailResponse>>(bookings);

                res.Message = $"Đã hủy {cancelledCount} đặt lịch tư vấn chưa thanh toán sau 24 giờ";

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy danh sách buổi tư vấn offline: {ex.Message}";
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetAllBookingOnlines()
        {
            var res = new ResultModel();
            try
            {
                var bookingOnlines = await _onlineRepo.GetBookingOnlinesRepo();
                if (bookingOnlines == null || !bookingOnlines.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsBooking.BOOKING_FOUND;
                res.Data = _mapper.Map<List<BookingOnlineDetailResponse>>(bookingOnlines);
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

        public async Task<ResultModel> GetAllBookingOfflines()
        {
            var res = new ResultModel();
            try
            {
                var bookingOfflines = await _offlineRepo.GetBookingOfflines();
                if (bookingOfflines == null || !bookingOfflines.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsBooking.BOOKING_FOUND;
                res.Data = _mapper.Map<List<BookingOfflineDetailResponse>>(bookingOfflines);
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

        public async Task<ResultModel> GetAllBookingByStaff()
        {
            var res = new ResultModel();
            try
            {
                var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var claims = identity.Claims;
                var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }
                var onlineBookings = await _onlineRepo.GetBookingsOnlineByStaffId(accountId) ?? new List<BookingOnline>();
                var offlineBookings = await _offlineRepo.GetBookingsOfflineByStaffId(accountId) ?? new List<BookingOffline>();

                if (onlineBookings == null && offlineBookings == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var allBookings = onlineBookings.Cast<object>().Concat(offlineBookings.Cast<object>()).ToList();

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Data = _mapper.Map<List<BookingResponse>>(allBookings);
                res.Message = ResponseMessageConstrantsBooking.BOOKING_FOUND;
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
