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
    }
}
