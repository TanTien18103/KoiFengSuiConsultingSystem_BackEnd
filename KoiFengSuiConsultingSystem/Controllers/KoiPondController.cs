using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Services.Interfaces;

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
        public async Task<ActionResult<object>> GetPondRecommendations()
        {
            var res = await _iKoiPondService.GetPondRecommendations();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get")]
        public async Task<ActionResult> GetPondById(string id)
        {
            var res = await _iKoiPondService.GetKoiPondById(id);
            return StatusCode(res.StatusCode, res);

        }

        [HttpGet("get-all")]
        public async Task<ActionResult> GetAllPond()
        {
            var res = await _iKoiPondService.GetAllKoiPonds();
            return StatusCode(res.StatusCode, res);
        }
    }
} 