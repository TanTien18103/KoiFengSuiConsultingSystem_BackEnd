using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Services.Services.KoiPondService;
using Services.ApiModels.KoiPond;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KoiPondController : ControllerBase
    {
        private readonly IKoiPondService _iKoiPondService;

        public KoiPondController(IKoiPondService iKoiPondService)
        {
            _iKoiPondService = iKoiPondService;
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("recommend")]
        public async Task<IActionResult> GetPondRecommendations()
        {
            var res = await _iKoiPondService.GetPondRecommendations();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPondById([FromRoute] string id)
        {
            var res = await _iKoiPondService.GetKoiPondById(id);
            return StatusCode(res.StatusCode, res);

        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllPonds()
        {
            var res = await _iKoiPondService.GetAllKoiPonds();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-all-shape")]
        public async Task<IActionResult> GetAllShapes()
        {
            var res = await _iKoiPondService.GetAllShapes();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-by-shape/{id}")]
        public async Task<IActionResult> GetPondsByShape([FromRoute] string id)
        {
            var res = await _iKoiPondService.GetKoiPondByShapeId(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateKoiPond([FromForm] KoiPondRequest koiPond)
        {
            var result = await _iKoiPondService.CreateKoiPond(koiPond);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKoiPond(string id, [FromBody] KoiPondRequest koiPond)
        {
            var result = await _iKoiPondService.UpdateKoiPond(id, koiPond);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKoiPond(string id)
        {
            var result = await _iKoiPondService.DeleteKoiPond(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-by-name")]
        public async Task<IActionResult> GetKoiPondsByName(string? name)
        {
            var res = await _iKoiPondService.GetKoiPondsByName(name);
            return StatusCode(res.StatusCode, res);
        }
    }
}