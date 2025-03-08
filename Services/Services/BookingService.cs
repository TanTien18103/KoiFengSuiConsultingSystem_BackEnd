using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.ApiModels;
using Services.ApiModels.BookingOnline;
using BusinessObjects.Enums;
using Services.Interfaces;
using Services.ApiModels.BookingOffline;
using Services.ApiModels.Booking;

namespace Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingOnlineRepo _onlineRepo;
        private readonly IBookingOfflineRepo _offlineRepo;
        private readonly IMapper _mapper;

        public BookingService(IBookingOnlineRepo repo, IMapper mapper, IBookingOfflineRepo offlineRepo)
        {
            _onlineRepo = repo;
            _mapper = mapper;
            _offlineRepo = offlineRepo;
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
