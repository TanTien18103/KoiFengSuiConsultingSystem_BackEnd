using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Workshop;
using Services.Services.WorkshopService;

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

        [HttpGet("sort-createdDate")]
        public async Task<IActionResult> SortingWorkshopByCreatedDate()
        {
            var res = await _workshopService.SortingWorkshopByCreatedDate();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("trending")]
        public async Task<IActionResult> GetTrendingWorkshop()
        {
            var res = await _workshopService.TrendingWorkshop();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("approve-workshop")]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkshopById([FromRoute] string id)
        {
            var result = await _workshopService.GetWorkshopById(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkshop([FromForm] WorkshopRequest request)
        {
            var result = await _workshopService.CreateWorkshop(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkshop([FromRoute] string id, [FromForm] WorkshopRequest request)
        {
            var result = await _workshopService.UpdateWorkshop(id, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkshop([FromRoute] string id)
        {
            var result = await _workshopService.DeleteWorkshop(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("check-in")]
        public async Task<IActionResult> CheckIn( string workshopId, string registerId)
        {
            var result = await _workshopService.CheckIn(workshopId, registerId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
