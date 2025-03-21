using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels;
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

        [HttpGet("get-by-name")]
        public async Task<IActionResult> GetKoiVarietiesByName(string name)
        {
            var res = await _koiVarietyService.GetKoiVarietiesByName(name);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-by-color")]
        public async Task<IActionResult> GetKoiVarietiesByColors([FromQuery] List<string> colorIds)
        {
            var result = await _koiVarietyService.GetKoiVarietiesByColorsAsync(colorIds);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-by-element")]
        public async Task<IActionResult> GetKoiVarietiesByElement(NguHanh element)
        {
            var res = await _koiVarietyService.GetKoiVarietiesByElementAsync(element);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterByColorAndElement(NguHanh? nguHanh = null, [FromQuery] List<string>? colorIds = null)
        {
            var res = await _koiVarietyService.FilterByColorAndElement(nguHanh, colorIds);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-all-colors")]
        public IActionResult GetAllColors()
        {
            var colors = Enum.GetValues(typeof(ColorEnums)).Cast<ColorEnums>().ToList();
            return Ok(colors);
        }

        [HttpGet("get-all-elements")]
        public IActionResult GetAllElements()
        {
            var elements = Enum.GetValues(typeof(NguHanh)).Cast<NguHanh>().ToList();
            return Ok(elements);
        }

        [HttpGet("api/colors-by-element/{nguHanh}")]
        public IActionResult GetColorsByElement(NguHanh nguHanh)
        {
            var colors = _koiVarietyService.GetPositiveColorsByElement(nguHanh);
            return Ok(colors);
        }

        [HttpGet("api/element-by-color/{color}")]
        public IActionResult GetElementByColor(ColorEnums color)
        {
            var element = _koiVarietyService.GetCompatibleElementsForColor(color);
            if (element == null)
            {
                return NotFound($"Không tìm thấy mệnh phù hợp với màu {color}");
            }
            return Ok(element);
        }

        //[HttpPost("api/check-colors-compatibility")]
        //public IActionResult CheckColorsCompatibility([FromBody] List<ColorEnums> colors)
        //{
        //    var result = _koiVarietyService.CheckColorsCompatibility(colors);
        //    return Ok(result);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetKoiVarietyById([FromRoute] string id)
        {
            var res = await _koiVarietyService.GetKoiVarietyWithColorsByIdAsync(id);
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
            var result = await _koiVarietyService.DeleteColors(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
