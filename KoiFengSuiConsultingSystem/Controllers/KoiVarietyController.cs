using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.KoiVarietyService;
using System.Xml.Linq;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KoiVarietyController : ControllerBase
    {
        private readonly IKoiVarietyService _koiVarietyService;

        public KoiVarietyController(IKoiVarietyService koiVarietyService)
        {
            _koiVarietyService = koiVarietyService;
        }

        [HttpGet("get-with-color")]
        public async Task<IActionResult> GetAllKoiVarieties()
        {
            var res = await _koiVarietyService.GetKoiVarietyWithColorsAsync();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetKoiVarietyById([FromRoute] string id)
        {
            var res = await _koiVarietyService.GetKoiVarietyWithColorsByIdAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-by-{element}")]
        public async Task<IActionResult> GetKoiVarietiesByElement([FromRoute]string element)
        {
            var res = await _koiVarietyService.GetKoiVarietiesByElementAsync(element);
            return StatusCode(res.StatusCode, res);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("get-koi-current-login")]
        public async Task<IActionResult> GetRecommendedKoiVarieties()
        {
            var res = await _koiVarietyService.GetRecommendedKoiVarietiesAsync();
            return StatusCode(res.StatusCode, res);
        }
    }
}
