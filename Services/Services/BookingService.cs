using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.ApiModels;
using Services.ApiModels.BookingOnline;
using BusinessObjects.Enums;
using Services.Interfaces;
using Services.ApiModels.BookingOffline;
using Services.ApiModels.Booking;
using BusinessObjects.Models;
using Repositories.Repository;

namespace Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingOnlineRepo _onlineRepo;
        private readonly IBookingOfflineRepo _offlineRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITransactionService _transactionService;
        private readonly ICustomerRepo _customerRepo;
        private readonly IAccountRepo _accountRepo;
        private readonly IMasterScheduleService _masterScheduleService;
        
        public BookingService(
            IBookingOnlineRepo onlineRepo,
            IBookingOfflineRepo offlineRepo,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ITransactionService transactionService,
            ICustomerRepo customerRepo,
            IAccountRepo accountRepo,
            IMasterScheduleService masterScheduleService
        )
        {
            _onlineRepo = onlineRepo;
            _offlineRepo = offlineRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _transactionService = transactionService;
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
                    res.Message = "Dữ liệu booking không được để trống";
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    res.IsSuccess = false;
                    res.Message = "Token xác thực không được cung cấp";
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var token = authHeader.Substring("Bearer ".Length);
                var accountId = await _accountRepo.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.Message = "Không thể xác thực tài khoản";
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var customer = await _customerRepo.GetCustomerByAccountId(accountId);
                if (string.IsNullOrEmpty(customer.CustomerId))
                {
                    res.IsSuccess = false;
                    res.Message = "Không tìm thấy thông tin khách hàng";
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                var booking = _mapper.Map<BookingOnline>(bookingOnlineRequest);
                booking.BookingOnlineId = GenerateShortGuid();
                booking.CustomerId = customer.CustomerId;
                booking.Status = BookingOnlineEnums.Pending.ToString();
                var createdBooking = await _onlineRepo.CreateBookingOnlineRepo(booking);

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

                // 🔹 Tạo giao dịch thanh toán
                var transactionResult = await _transactionService.CreateTransactionWithDocNo(
                    createdBooking.BookingOnlineId,
                    bookingOnlineRequest.PaymentMethod,
                    "Booking Online"
                );

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status201Created;
                res.Message = "Tạo booking thành công";
                res.Data = new
                {
                    BookingOnline = _mapper.Map<BookingOnlineDetailResponse>(createdBooking),
                    Transaction = transactionResult.Data
                };
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
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
                if (string.IsNullOrEmpty(bookingId))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = "ID booking không được để trống";
                    return res;
                }

                var onlineBooking = await _onlineRepo.GetBookingOnlineByIdRepo(bookingId);
                if (onlineBooking != null)
                {
                    res.IsSuccess = true;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<BookingOnlineDetailResponse>(onlineBooking);
                    res.Message = "Lấy thông tin booking online thành công";
                    return res;
                }

                var offlineBooking = await _offlineRepo.GetBookingOfflineById(bookingId);
                if (offlineBooking != null)
                {
                    res.IsSuccess = true;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<BookingOfflineDetailResponse>(offlineBooking);
                    res.Message = "Lấy thông tin booking offline thành công";
                    return res;
                }

                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = $"Không tìm thấy booking nào với ID: {bookingId}";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
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
                if (string.IsNullOrEmpty(masterScheduleId))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = "ID consulting không được để trống";
                    return res;
                }

                var onlineBooking = await _onlineRepo.GetConsultingOnlineByMasterScheduleIdRepo(masterScheduleId);
                if (onlineBooking != null)
                {
                    res.IsSuccess = true;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<ConsultingOnlineDetailResponse>(onlineBooking);
                    res.Message = "Lấy thông tin booking online thành công";
                    return res;
                }

                var offlineBooking = await _offlineRepo.GetConsultingOfflineByMasterScheduleIdRepo(masterScheduleId);
                if (offlineBooking != null)
                {
                    res.IsSuccess = true;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<ConsultingOfflineDetailResponse>(offlineBooking);
                    res.Message = "Lấy thông tin booking offline thành công";
                    return res;
                }

                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = $"Không tìm thấy consulting nào với ID masterSchedule: {masterScheduleId}";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
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
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = $"Không tìm thấy các buổi tư vấn online với trạng thái {status}";
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
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = $"Không tìm thấy các buổi tư vấn offline với trạng thái {status}";
                        return res;
                    }
                }

                if (!bookingList.Any())
                {
                    string typeMessage = type.HasValue ? (type == BookingTypeEnums.Online ? "online" : "offline") : "online và offline";
                    string statusMessage = status.HasValue ? $"với trạng thái {status}" : "";

                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = $"Không tìm thấy các buổi tư vấn {typeMessage} {statusMessage}";
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = bookingList;

                string typeSuccessMessage = type.HasValue ? (type == BookingTypeEnums.Online ? "online" : "offline") : "online và offline";
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
                if (string.IsNullOrEmpty(bookingId) || string.IsNullOrEmpty(masterId))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Booking ID và Master ID không được để trống",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                var booking = await _onlineRepo.GetBookingOnlineByIdRepo(bookingId);
                if (booking == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = $"Không tìm thấy booking với ID: {bookingId}",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if (!string.IsNullOrEmpty(booking.MasterId))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = "Booking này đã có Master",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                booking.MasterId = masterId;
                await _onlineRepo.UpdateBookingOnlineRepo(booking);

                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Gán Master cho Booking thành công",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi gán Master: {ex.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> Calculate(BookingTypeEnums bookingType, string bookingId)
        {
            var res = new ResultModel();
            if (string.IsNullOrEmpty(bookingId))
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status400BadRequest;
                res.Message = "Invalid booking ID";
                return res;
            }

            switch (bookingType)
            {
                case BookingTypeEnums.Online:
                    var onlineBooking = await _onlineRepo.GetBookingOnlineByIdRepo(bookingId);
                    if (onlineBooking == null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = "Online booking not found";
                        return res;
                    }

                    var onlineResponse = _mapper.Map<BookingResponse>(onlineBooking);
                    // Type is already set by the mapper profile

                    res.IsSuccess = true;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = onlineResponse;
                    res.Message = "Successfully retrieved online booking";
                    return res;

                case BookingTypeEnums.Offline:
                    var offlineBooking = await _offlineRepo.GetBookingOfflineById(bookingId);
                    if (offlineBooking == null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = "Offline booking not found";
                        return res;
                    }

                    var offlineResponse = _mapper.Map<BookingResponse>(offlineBooking);
                    // Type is already set by the mapper profile

                    res.IsSuccess = true;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = offlineResponse;
                    res.Message = "Successfully retrieved offline booking";
                    return res;

                default:
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = "Invalid booking type";
                    return res;
            }
        }
    }
}
