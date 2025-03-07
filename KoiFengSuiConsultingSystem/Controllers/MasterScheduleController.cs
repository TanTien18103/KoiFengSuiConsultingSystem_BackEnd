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
            var res = await _masterScheduleService.GetAllMasterSchedules();
            return StatusCode(res.StatusCode, res); 
        }

        [HttpGet("{masterId}")]
        public async Task<IActionResult> GetMasterSchedulesByMasterId(string masterId)
        {
            var res = await _masterScheduleService.GetMasterSchedulesByMasterId(masterId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("{masterId}/{date}")]
        public async Task<IActionResult> GetMasterSchedulesByMasterIdGetMasterSchedulesByMasterAndDate(string masterId, DateTime date)
        {
            var res = await _masterScheduleService.GetMasterSchedulesByMasterAndDate(masterId, date);
            return StatusCode(res.StatusCode, res);
        }

    }
} 