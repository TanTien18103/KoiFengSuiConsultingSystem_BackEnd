using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.MasterScheduleService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterScheduleController : ControllerBase
    {
        private readonly IMasterScheduleService _masterScheduleService;

        public MasterScheduleController(IMasterScheduleService masterScheduleService)
        {
            _masterScheduleService = masterScheduleService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllSchedules()
        {
            var res = await _masterScheduleService.GetAllMasterSchedules();
            return StatusCode(res.StatusCode, res); 
        }

        [Authorize(Roles = "Master")]
        [HttpGet("get-schedule-by-current-master")]
        public async Task<IActionResult> GetMasterSchedulesByMasterId()
        {
            var res = await _masterScheduleService.GetMasterSchedulesByCurrentMasterLogin();
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("{masterId}/{date}")]
        public async Task<IActionResult> GetMasterSchedulesByMasterIdGetMasterSchedulesByMasterAndDate([FromRoute]string masterId, [FromRoute] DateTime date)
        {
            var res = await _masterScheduleService.GetMasterSchedulesByMasterAndDate(masterId, date);
            return StatusCode(res.StatusCode, res);
        }
    }
} 