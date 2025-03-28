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
        [HttpGet("master/booking-onlines")]
        [Authorize(Roles = "Master")]
        public async Task<IActionResult> GetBookingOnlinesByMaster()
        {
            var result = await _bookingService.GetBookingOnlinesByMaster();
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
        [HttpGet("master/booking-offlines")]
        [Authorize(Roles = "Master")]
        public async Task<IActionResult> GetBookingOfflinesByMaster()
        {
            var result = await _bookingService.GetBookingOfflinesByMaster();

        [HttpGet("cancel-unpaid")]
        [Authorize(Roles = "Master")]
        public async Task<IActionResult> CancelUnpaidBookings()
        {
            var result = await _bookingService.CancelUnpaidBookings();
            return StatusCode(result.StatusCode, result);
        }
    }
}
