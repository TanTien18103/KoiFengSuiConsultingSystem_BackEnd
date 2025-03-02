using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

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
            var result = await _koiVarietyService.GetKoiVarietyWithColors();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetKoiVarietyById(string id)
        {
            var result = await _koiVarietyService.GetKoiVarietyWithColorsById(id);

            if (result == null)
                return NotFound($"Can not find: {id}");

            return Ok(result);
        }

    }
}
