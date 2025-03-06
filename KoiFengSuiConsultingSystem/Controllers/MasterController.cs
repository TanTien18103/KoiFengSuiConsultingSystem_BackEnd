using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Services;

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

        [HttpGet("get-all-masters")]
        public async Task<IActionResult> GetAllMasters()
        {
            var res = await _iMasterService.GetAllMasters();
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get-master")]
        public async Task<IActionResult> GetMasterById(string id)
        {
            var res = await _iMasterService.GetMasterById(id);
            return StatusCode(res.StatusCode, res);

        }
    }
    }
