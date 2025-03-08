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
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingOnlineService;

        public BookingController(IBookingService bookingOnlineService)
        {
            _bookingOnlineService = bookingOnlineService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById([FromRoute] string id)
        {
            var res = await _bookingOnlineService.GetBookingByIdAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-booking-online")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetBookingOnline([FromQuery] BookingOnlineEnums? status = null, [FromQuery] BookingTypeEnums? type = null)
        {
            var res = await _bookingOnlineService.GetBookingByStatusAsync(status, type);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("online-Hover")]
        public async Task<IActionResult> GetBookingOnlines()
        {
            var res = await _bookingOnlineService.GetBookingOnlinesHoverAsync();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("assign-master")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> AssignMaster([FromQuery] string bookingId, [FromQuery] string masterId)
        {
            var result = await _bookingOnlineService.AssignMasterToBookingAsync(bookingId, masterId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
