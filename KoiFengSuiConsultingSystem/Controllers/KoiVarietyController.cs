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
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllKoiVarietyById([FromRoute]string id)
        {
            var res = await _koiVarietyService.GetKoiVarietyByIdAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllKoiVarieties()
        {
            var res = await _koiVarietyService.GetKoiVarietiesAsync();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-by-name")]
        public async Task<IActionResult> GetKoiVarietiesByName(string? name)
        {
            var res = await _koiVarietyService.GetKoiVarietiesByName(name);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-by-color")]
        public async Task<IActionResult> GetKoiVarietiesByColors([FromQuery] List<ColorEnums> colors)
        {
            var result = await _koiVarietyService.GetKoiVarietiesByColorsAsync(colors);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-by-element")]
        public async Task<IActionResult> GetKoiVarietiesByElement(NguHanh element)
        {
            var res = await _koiVarietyService.GetKoiVarietiesByElementAsync(element);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterKoiVarieties([FromQuery] NguHanh? nguHanh = null, [FromQuery] List<ColorEnums>? colors = null)
        {
            if (colors != null && colors.Contains(default(ColorEnums)))
            {
                colors = null;
            }

            var result = await _koiVarietyService.FilterByColorAndElement(nguHanh, colors);
            return StatusCode(result.StatusCode, result);
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

        [HttpGet("compatible-elements")]
        public IActionResult GetCompatibleElements([FromQuery] string colors)
        {
            if (string.IsNullOrEmpty(colors))
            {
                return BadRequest(new { success = false, message = ResponseMessageConstrantsKoiVariety.COLOR_INPUT_REQUIRED });
            }

            // Chuyển đổi chuỗi colors thành List<ColorEnums>
            var colorList = colors.Split(',')
                                 .Select(c => c.Trim())
                                 .Where(c => !string.IsNullOrEmpty(c))
                                 .Select(c => {
                                     if (Enum.TryParse<ColorEnums>(c, true, out var colorEnum))
                                         return (success: true, color: colorEnum);
                                     return (success: false, color: default(ColorEnums));
                                 })
                                 .Where(result => result.success)
                                 .Select(result => result.color)
                                 .ToList();

            if (colorList.Count == 0)
            {
                return BadRequest(new { success = false, message = "Invalid color values provided" });
            }

            // Khai báo kiểu dữ liệu rõ ràng cho các biến
            bool isCompatible;
            List<NguHanh> elements;
            string message;

            // Sử dụng deconstruction với kiểu dữ liệu rõ ràng
            (isCompatible, elements, message) = _koiVarietyService.GetCompatibleElementsForColors(colorList);

            return Ok(new
            {
                success = isCompatible,
                elements = elements.Select(e => e.ToString()).ToList(),
                message = message
            });
        }

        [HttpGet("with-color-by/{id}")]
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
