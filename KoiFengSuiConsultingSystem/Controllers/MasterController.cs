using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Services.Services.MasterService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private readonly IMasterService _iMasterService;
        public MasterController(IMasterService iMasterService)
        {
            _iMasterService = iMasterService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllMasters()
        {
            var res = await _iMasterService.GetAllMasters();
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMasterById([FromRoute]string id)
        {
            var res = await _iMasterService.GetMasterById(id);
            return StatusCode(res.StatusCode, res);
        }
    }
}
