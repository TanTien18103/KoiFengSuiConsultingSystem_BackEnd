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
        public async Task<IActionResult> GetPondById([FromRoute]string id)
        {
            var res = await _iKoiPondService.GetKoiPondById(id);
            return StatusCode(res.StatusCode, res);

        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllPond()
        {
            var res = await _iKoiPondService.GetAllKoiPonds();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateKoiPond([FromBody] KoiPondRequest koiPond)
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
    }
} 