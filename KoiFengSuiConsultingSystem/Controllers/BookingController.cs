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

        [HttpPut("assign-master")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> AssignMaster( string bookingId, string masterId)
        {
            var result = await _bookingService.AssignMasterToBookingAsync(bookingId, masterId);
            return StatusCode(result.StatusCode, result);
        }


        // Booking Offline
        [HttpPost("offline-create")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateBookingOffline([FromBody]BookingOfflineRequest request)
        {
            var res = await _bookingService.CreateBookingOffline(request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("offline-add/{packageId}-to/{offlineId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddPackage([FromRoute]string packageId, [FromRoute] string offlineId)
        {
            var res = await _bookingService.AddConsultationPackage(packageId, offlineId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("offline-remove-package/{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemovePackage([FromRoute] string id)
        {
            var res = await _bookingService.RemoveConsultationPackage(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("offline-select-price/{id}/{selectedPrice}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> SelectPrice([FromRoute] string id, [FromRoute] decimal selectedPrice)
        {
            var res = await _bookingService.SelectBookingOfflinePrice(id, selectedPrice);
            return StatusCode(res.StatusCode, res);
        }
    }
}
