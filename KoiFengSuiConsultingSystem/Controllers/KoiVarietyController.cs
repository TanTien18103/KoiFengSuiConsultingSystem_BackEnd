using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
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

        [HttpGet]
        public async Task<IActionResult> GetAllKoiVarieties()
        {
            var result = await _koiVarietyService.GetKoiVarietyWithColorsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetKoiVarietyById(string id)
        {
            var result = await _koiVarietyService.GetKoiVarietyWithColorsByIdAsync(id);

            if (result == null)
                return NotFound($"Can not find: {id}");

            return Ok(result);
        }

        [HttpGet("get-by-element/{element}")]
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
