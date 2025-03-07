using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.ApiModels;
using Services.ApiModels.BookingOnline;
using Services.ApiModels.KoiVariety;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Repositories.Interfaces;
using Services.ApiModels;
using Services.ApiModels.BookingOnline;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Repository;
using Services.ApiModels.BookingOffline;

namespace Services.Services
{
    public class BookingOnlineService : IBookingOnlineService
    {
        private readonly IBookingOnlineRepo _onlineRepo;
        private readonly IBookingOfflineRepo _offlineRepo;
        private readonly IMapper _mapper;
        private readonly IBookingTypeService _bookingTypeService;

        public BookingOnlineService(IBookingOnlineRepo repo, IMapper mapper, IBookingTypeService bookingTypeService, IBookingOfflineRepo offlineRepo)
        {
            _onlineRepo = repo;
            _mapper = mapper;
            _bookingTypeService = bookingTypeService;
            _offlineRepo = offlineRepo;
        }

        public async Task<ResultModel> AssignMasterToBooking(string bookingId, string masterId)
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

        public async Task<ResultModel> GetBookingOnlineById(string bookingId)
        {
            var res = new ResultModel();
            try
            {
                if (string.IsNullOrEmpty(bookingId))
                {
                    res.IsSuccess = false;
                    res.Message = "ID booking không được để trống";
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var booking = await _onlineRepo.GetBookingOnlineByIdRepo(bookingId);
                if (booking == null)
                {
                    res.IsSuccess = false;
                    res.Message = $"Không tìm thấy booking với ID: {bookingId}";
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<BookingOnlineDetailRespone>(booking);
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
                List<object> bookingList = new List<object>();

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
                        foreach (var booking in filteredBookings)
                        {
                            var calculateResult = await _bookingTypeService.Calculate(BookingTypeEnums.Online, booking.BookingOnlineId);
                            if (calculateResult.IsSuccess && calculateResult.Data is BookingOnlineDetailRespone response)
                            {
                                bookingList.Add(response);
                            }
                        }
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
                        foreach (var booking in filteredBookings)
                        {
                            var calculateResult = await _bookingTypeService.Calculate(BookingTypeEnums.Offline, booking.BookingOfflineId);
                            if (calculateResult.IsSuccess && calculateResult.Data is BookingOfflineResponse response)
                            {
                                bookingList.Add(response);
                            }
                        }
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

        public async Task<ResultModel> GetBookingOnlinesHover()
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
    }
}
