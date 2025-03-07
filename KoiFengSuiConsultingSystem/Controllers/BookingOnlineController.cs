using Microsoft.AspNetCore.Http;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingOnlineController : ControllerBase
    {
        private readonly IBookingOnlineService _bookingOnlineService;

        public BookingOnlineController(IBookingOnlineService bookingOnlineService)
        {
            _bookingOnlineService = bookingOnlineService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingOnlines()
        {
            var res = await _bookingOnlineService.GetBookingOnlines();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetBookingOnlineById([FromRoute]string id)
        {
            var res = await _bookingOnlineService.GetBookingOnlineById(id);
            return StatusCode(res.StatusCode, res);
        }


        [HttpGet("get-booking-online")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetHistoryBookingOnline([FromQuery] BookingOnlineEnums? status = null)
        {
            var res = await _bookingOnlineService.GetBookingOnlineByStatusAsync(status);
            return StatusCode(res.StatusCode, res);
        }
    }
}
