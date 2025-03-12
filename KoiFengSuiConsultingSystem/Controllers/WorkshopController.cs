using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Workshop;
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

        [HttpGet]
        public async Task<IActionResult> GetWorkshops()
        {
            var result = await _workshopService.GetWorkshop();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkshopById(string id)
        {
            var result = await _workshopService.GetWorkshopById(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkshop([FromBody] WorkshopRequest request)
        {
            var result = await _workshopService.CreateWorkshop(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkshop(string id, [FromBody] WorkshopRequest request)
        {
            var result = await _workshopService.UpdateWorkshop(id, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkshop(string id)
        {
            var result = await _workshopService.DeleteWorkshop(id);
            return StatusCode(result.StatusCode, result);
        }

    }
}
