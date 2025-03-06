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
            var result = await _bookingOnlineService.GetBookingOnlines();
            return Ok(result);
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingOnlineById(string bookingId)
        {
            var result = await _bookingOnlineService.GetBookingOnlineById(bookingId);
            return Ok(result);
        }

    }
}
