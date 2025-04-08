using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.ConsultationPackage;
using Services.Services.ConsultationPackageService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultationPackageController : ControllerBase
    {
        private readonly IConsultationPackageService _consultationPackageService;
        public ConsultationPackageController(IConsultationPackageService consultationPackageService)
        {
            _consultationPackageService = consultationPackageService;
        }

        [HttpGet("get-by/{id}")]
        public async Task<IActionResult> GetPackageById([FromRoute]string id)
        {
            var res = await _consultationPackageService.GetConsultationPackageById(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetPackages()
        {
            var res = await _consultationPackageService.GetConsultationPackages();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePackage([FromForm] ConsultationPackageRequest consultationPackageRequest)
        {
            var res = await _consultationPackageService.CreateConsultationPackage(consultationPackageRequest);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePackage([FromRoute] string id, [FromForm] ConsultationPackageUpdateRequest consultationPackageRequest)
        {
            var res = await _consultationPackageService.UpdateConsultationPackage(id, consultationPackageRequest);
            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePackage([FromRoute] string id)
        {
            var res = await _consultationPackageService.DeleteConsultationPackage(id);
            return StatusCode(res.StatusCode, res);
        }
    }
}
