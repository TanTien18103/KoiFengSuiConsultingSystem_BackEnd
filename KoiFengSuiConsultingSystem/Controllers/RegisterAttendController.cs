using BusinessObjects.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterAttendController : ControllerBase
    {
        private readonly IRegisterAttendService _registerAttendService;
        public RegisterAttendController(IRegisterAttendService registerAttendService)
        {
            _registerAttendService = registerAttendService;
        }

        [HttpGet("get-register-attend")]
        public async Task<IActionResult> GetRegisterAttends([FromQuery] RegisterAttendStatusEnums? status = null)
        {
            var res = await _registerAttendService.GetRegisterAttends(status);
            return StatusCode(res.StatusCode, res);
        }


      
        [HttpGet("customer")]
        public async Task<IActionResult> GetRegisterAttendByCustomerId()
        {
            var result = await _registerAttendService.GetRegisterAttendByCustomerId();
            return StatusCode(result.StatusCode, result);
        }

        
        [HttpGet("{registerAttendId}")]
        public async Task<IActionResult> GetRegisterAttendById(string registerAttendId)
        {
            var result = await _registerAttendService.GetRegisterAttendById(registerAttendId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRegisterAttends()
        {
            var result = await _registerAttendService.GetRegisterAttends();
            return StatusCode(result.StatusCode, result);
        }

      
        [HttpGet("workshop/{id}")]
        public async Task<IActionResult> GetRegisterAttendByWorkshopId(string id)
        {
            var result = await _registerAttendService.GetRegisterAttendByWorkshopId(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
