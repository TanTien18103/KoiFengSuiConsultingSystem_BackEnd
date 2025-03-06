using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.ApiModels;
using Services.ApiModels.BookingOnline;
using Services.ApiModels.KoiVariety;
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

        public async Task<ResultModel<BookingOnlineDetailResponeDTO>> GetBookingOnlineById(string bookingId)
        {
            var res = new ResultModel<BookingOnlineDetailResponeDTO>();
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
                res.Data = _mapper.Map<BookingOnlineDetailResponeDTO>(booking);
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

        public async Task<ResultModel<List<BookingOnlineHoverResponeDTO>>> GetBookingOnlines()
        {
            var res = new ResultModel<List<BookingOnlineHoverResponeDTO>>();
            try
            {
                var bookings = await _repo.GetBookingOnlinesRepo();
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<BookingOnlineHoverResponeDTO>>(bookings);
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
