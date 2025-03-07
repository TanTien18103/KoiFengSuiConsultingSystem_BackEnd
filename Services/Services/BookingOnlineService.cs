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

namespace Services.Services
{
    public class BookingOnlineService : IBookingOnlineService
    {
        private readonly IBookingOnlineRepo _repo;
        private readonly IMapper _mapper;

        public BookingOnlineService(IBookingOnlineRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
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

                var booking = await _repo.GetBookingOnlineByIdRepo(bookingId);
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
        
        public async Task<ResultModel> GetBookingOnlineByStatusAsync(BookingOnlineEnums? status)
        {
            var res = new ResultModel();
            try
            {
                var bookingOnline = await _repo.GetBookingOnlinesRepo();
                var filteredBookings = bookingOnline;

                if (status.HasValue)
                {
                    filteredBookings = bookingOnline.Where(x => x.Status == status.ToString()).ToList();

                    if (!filteredBookings.Any())
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = $"Không tìm thấy các buổi tư vấn online với trạng thái {status}";
                        return res;
                    }
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<BookingOnlineDetailRespone>>(filteredBookings);
                string statusMessage = status.HasValue ? $"với trạng thái {status}" : "";
                res.Message = $"Lấy danh sách buổi tư vấn {statusMessage} thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message =  ex.Message;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetBookingOnlines()
        {
            var res = new ResultModel();
            try
            {
                var bookings = await _repo.GetBookingOnlinesRepo();
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
