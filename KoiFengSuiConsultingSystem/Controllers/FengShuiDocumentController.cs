using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repositories.FengShuiDocumentRepository;
using Services.Services.FengShuiDocumentService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FengShuiDocumentController : ControllerBase
    {
        private readonly IFengShuiDocumentService _fengShuiDocumentService;
        public FengShuiDocumentController(IFengShuiDocumentService fengShuiDocumentService)
        {
            _fengShuiDocumentService = fengShuiDocumentService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFengShuiDocumentById([FromRoute] string id)
        {
            var result = await _fengShuiDocumentService.GetFengShuiDocumentById(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllFengShuiDocuments()
        {
            var result = await _fengShuiDocumentService.GetAllFengShuiDocuments();
            return StatusCode(result.StatusCode, result);
        }
    }
}
