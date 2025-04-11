using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Location;
using Services.Services.LocationService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocationById([FromRoute] string id)
        {
            var res = await _locationService.GetLocationById(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllLocations()
        {
            var res = await _locationService.GetAllLocations();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLocation([FromForm] LocationRequest location)
        {
            var res = await _locationService.CreateLocation(location);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation([FromRoute] string id, [FromForm] LocationUpdateRequest location)
        {
            var res = await _locationService.UpdateLocation(id, location);
            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation([FromRoute] string id)
        {
            var res = await _locationService.DeleteLocation(id);
            return StatusCode(res.StatusCode, res);
        }
    }
}
