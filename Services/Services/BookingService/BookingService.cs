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
        private readonly IMasterScheduleService _masterScheduleService;

        public BookingService(
            IBookingOnlineRepo onlineRepo,
            IBookingOfflineRepo offlineRepo,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ICustomerRepo customerRepo,
            IAccountRepo accountRepo,
            IMasterScheduleService masterScheduleService
        )
        {
            _onlineRepo = onlineRepo;
            _offlineRepo = offlineRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _customerRepo = customerRepo;
            _accountRepo = accountRepo;
            _masterScheduleService = masterScheduleService;
        }
        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }

        public async Task<ResultModel> CreateBookingOnline(BookingOnlineRequest bookingOnlineRequest)
        {
            var res = new ResultModel();
            try
            {
                if (bookingOnlineRequest == null)
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

                var masterSchedule = new MasterSchedule
                {
                    MasterScheduleId = GenerateShortGuid(),
                    MasterId = bookingOnlineRequest.MasterId,
                    Date = bookingOnlineRequest.BookingDate,
                    StartTime = bookingOnlineRequest.StartTime,
                    EndTime = bookingOnlineRequest.EndTime,
                    Type = "Booking Online",
                    Status = "Pending"
                };
                await _masterScheduleService.CreateMasterSchedule(masterSchedule);

                var booking = _mapper.Map<BookingOnline>(bookingOnlineRequest);
                booking.BookingOnlineId = GenerateShortGuid();
                booking.CustomerId = customer.CustomerId;
                booking.Status = BookingOnlineEnums.Pending.ToString();
                booking.MasterScheduleId = masterSchedule.MasterScheduleId;
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

        public async Task<ResultModel> AssignMasterToBookingAsync(string bookingId, string masterId)
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

                var booking = await _onlineRepo.GetBookingOnlineByIdRepo(bookingId);
                if (booking == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                if (!string.IsNullOrEmpty(booking.MasterId))
                {

                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.EXISTED;
                    res.Message = ResponseMessageConstrantsBooking.ALREADY_ASSIGNED;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                booking.MasterId = masterId;
                booking.AssignStaffId = accountId;
                await _onlineRepo.UpdateBookingOnlineRepo(booking);

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
    }
}
