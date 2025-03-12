using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkshopController : ControllerBase
    {
        private readonly IWorkshopService _workshopService;
        public WorkshopController(IWorkshopService workshopService)
        {
            _workshopService = workshopService;
        }

        [HttpGet("list-by-created-date")]
        public async Task<IActionResult> SortingWorkshopByCreatedDate()
        {
            var res = await _workshopService.SortingWorkshopByCreatedDate();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("trending-workshop")]
        public async Task<IActionResult> GetTrendingWorkshop([FromQuery]bool? trening = null)
        {
            var res = await _workshopService.TrendingWorkshop(trening);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("aprove-workshop")]
        public async Task<IActionResult> ApprovedWorkshop(string id)
        {
            var res = await _workshopService.ApprovedWorkshop(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("reject-workshop")]
        public async Task<IActionResult> RejectedWorkshop(string id)
        {
            var res = await _workshopService.RejectedWorkshop(id);
            return StatusCode(res.StatusCode, res);
        }

    }
}
