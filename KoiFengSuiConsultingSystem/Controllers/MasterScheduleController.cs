using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

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

        [HttpGet]
        public async Task<IActionResult> GetAllSchedules()
        {
            var schedules = await _masterScheduleService.GetAllMasterSchedulesAsync();
            return Ok(schedules);
        }

        [HttpGet("{masterId}")]
        public async Task<IActionResult> GetMasterSchedules(string masterId, [FromQuery] DateTime date)
        {
            var schedules = await _masterScheduleService.GetMasterSchedulesAsync(masterId, date);
            return Ok(schedules);
        }
    }
} 