using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.RegisterCourseService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterCourseController : ControllerBase
    {
        private readonly IRegisterCourseService _registerCourseService;

        public RegisterCourseController(IRegisterCourseService registerCourseService)
        {
            _registerCourseService = registerCourseService;
        }

        [HttpPut("{chapterId}")]
        public async Task<IActionResult> UpdateUserCourseStatus(string chapterId)
        {
            var result = await _registerCourseService.UpdateUserCourseStatus(chapterId);
            return Ok(result);
        }
    }
}
