using Microsoft.AspNetCore.Http;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.BookingOnline;
using Services.Services.BookingService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBookingOnline([FromBody] BookingOnlineRequest bookingOnlineRequest)
        {
            var res = await _bookingService.CreateBookingOnline(bookingOnlineRequest);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById([FromRoute] string id)
        {
            var res = await _bookingService.GetBookingByIdAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("consulting-by-masterSchedule-{id}")]
        public async Task<IActionResult> GetConsultingDetailByMasterScheduleId([FromRoute] string id)
        {
            var res = await _bookingService.GetConsultingDetailByMasterScheduleIdAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("status-type")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetBookingOnline([FromQuery] BookingOnlineEnums? status = null, [FromQuery] BookingTypeEnums? type = null)
        {
            var res = await _bookingService.GetBookingByStatusAsync(status, type);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("online-Hover")]
        public async Task<IActionResult> GetBookingOnlines()
        {
            var res = await _bookingService.GetBookingOnlinesHoverAsync();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("assign-master-{bookingId}-{masterId}")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> AssignMaster([FromRoute] string bookingId, [FromRoute] string masterId)
        {
            var result = await _bookingService.AssignMasterToBookingAsync(bookingId, masterId);
            return StatusCode(result.StatusCode, result);
        }


    }
}
