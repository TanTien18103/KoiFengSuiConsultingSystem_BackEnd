using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Services.Interface;

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

        [Authorize]
        [HttpGet("recommend")]
        public async Task<ActionResult<object>> GetPondRecommendations()
        {
            try
            {
                var result = await _iKoiPondService.GetPondRecommendations();
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("get")]
        public async Task<ActionResult> GetPondById(string id)
        {
            try
            {
                var result = await _iKoiPondService.GetKoiPondById(id);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("get-all")]
        public async Task<ActionResult> GetAllPond()
        {
            try
            {
                var result = await _iKoiPondService.GetAllKoiPonds();
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
    }
} 