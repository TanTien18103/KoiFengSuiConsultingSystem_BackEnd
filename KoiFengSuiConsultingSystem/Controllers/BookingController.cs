using Microsoft.AspNetCore.Http;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.BookingOnline;
using Services.Services.BookingService;
using Services.ApiModels.BookingOffline;

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

        [HttpGet("get-All-BookingTypeEnums")]
        public IActionResult GetAllBookingTypeEnums()
        {
            var types = Enum.GetValues(typeof(BookingTypeEnums))
                            .Cast<BookingTypeEnums>()
                            .Select(e => new { Id = (int)e, Name = e.ToString() })
                            .ToList();
            return Ok(types);
        }

        [HttpGet("get-All-BookingOnlineEnums")]
        public IActionResult GetAllBookingOnlineEnums()
        {
            var types = Enum.GetValues(typeof(BookingOnlineEnums))
                            .Cast<BookingOnlineEnums>()
                            .Select(e => new { Id = (int)e, Name = e.ToString() })
                            .ToList();
            return Ok(types);
        }

        [HttpGet("get-All-BookingOfflineEnums")]
        public IActionResult GetAllBookingOfflineEnums()
        {
            var types = Enum.GetValues(typeof(BookingOfflineEnums))
                            .Cast<BookingOfflineEnums>()
                            .Select(e => new { Id = (int)e, Name = e.ToString() })
                            .ToList();
            return Ok(types);
        }

        // Booking Online
        [HttpPost("create")]
        public async Task<IActionResult> CreateBookingOnline([FromBody] BookingOnlineRequest bookingOnlineRequest)
        {
            var result = await _bookingService.CreateBookingOnline(bookingOnlineRequest);
            return StatusCode(result.StatusCode, result);
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

        [HttpGet("current-login-bookingOffline")]
        public async Task<IActionResult> GetBookingOfflineForCurrentLogin()
        {
            var res = await _bookingService.GetBookingOfflineForCurrentLogin();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("assign-master")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> AssignMaster( string? bookingonline, string? bookingoffline, string masterId)
        {
            var result = await _bookingService.AssignMasterToBookingAsync(bookingonline,bookingoffline,masterId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut("booking-online/{bookingOnlineId}/complete")]
        [Authorize(Roles = "Master")]
        public async Task<IActionResult> CompleteBookingOnline(string bookingOnlineId)
        {
            var result = await _bookingService.CompleteBookingOnlineByMaster(bookingOnlineId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut("booking-online/{bookingOnlineId}/master-note")]
        [Authorize(Roles = "Master")]
        public async Task<IActionResult> UpdateBookingOnlineMasterNote(string bookingOnlineId, [FromBody] UpdateMasterNoteRequest request)
        {
            var result = await _bookingService.UpdateBookingOnlineMasterNote(bookingOnlineId, request);
            return StatusCode(result.StatusCode, result);
        }
        // Booking Offline
        [HttpPut("offline-remove-package/{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemovePackage([FromRoute] string id)
        {
            var res = await _bookingService.RemoveConsultationPackage(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("offline-transaction-complete")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ProcessCompleteBooking([FromBody] BookingOfflineRequest request, string packageId, decimal selectedPrice)
        {
            var res = await _bookingService.ProcessCompleteBooking(request, packageId, selectedPrice);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-bookings-by-type-and-status")]
        public async Task<IActionResult> GetBookings(
       [FromQuery] BookingTypeEnums? type,
       [FromQuery] BookingOnlineEnums? onlineStatus,
       [FromQuery] BookingOfflineEnums? offlineStatus)
        {
            var result = await _bookingService.GetBookingByTypeAndStatus(type, onlineStatus, offlineStatus);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);

            return Ok(result);
        }
    }
}
