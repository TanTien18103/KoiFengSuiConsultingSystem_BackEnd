using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.KoiVariety;
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateKoiVariety([FromBody] KoiVarietyRequest request)
        { 

            var result = await _koiVarietyService.CreateKoiVarietyAsync(request);
            return StatusCode(result.StatusCode, result);
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKoiVariety(string id, [FromBody] KoiVarietyRequest request)
        {
            var result = await _koiVarietyService.UpdateKoiVarietyAsync(id, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKoiVariety(string id)
        {
            var result = await _koiVarietyService.DeleteKoiVarietyAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-color-by-id/{id}")]
        public async Task<IActionResult> GetColorById(string id)
        {
            var result = await _koiVarietyService.GetColorById(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-colors")]
        public async Task<IActionResult> GetColors()
        {
            var result = await _koiVarietyService.GetColors();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("create-color")]
        public async Task<IActionResult> CreateColor([FromBody] ColorRequest colorRequest)
        {
            var result = await _koiVarietyService.CreateColors(colorRequest);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update-color/{id}")]
        public async Task<IActionResult> UpdateColor(string id, [FromBody] ColorRequest colorRequest)
        {
            var result = await _koiVarietyService.UpdateColors(id, colorRequest);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("delete-color/{id}")]
        public async Task<IActionResult> DeleteColor(string id)
        {
            var result = await  _koiVarietyService.DeleteColors(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
