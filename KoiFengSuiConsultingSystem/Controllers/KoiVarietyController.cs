using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

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
        //[HttpGet("match")]
        //public async Task<ActionResult<List<KoiVarietyElementDTO>>> GetMatchingKoiVarieties(Customer customer)
        //{
        //    var matches = await _koiVarietyService.GetKoiVarietiesByCustomerElementAsync(customer);
        //    if (!matches.Any())
        //    {
        //        return NotFound("No matching koi varieties found for your element.");
        //    }
        //    return Ok(matches);
        //}

    }
}
