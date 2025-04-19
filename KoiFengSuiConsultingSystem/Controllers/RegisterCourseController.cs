using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Certificate;
using Services.ApiModels.RegisterCourse;
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

        [HttpPut("submit-answers-by/{quizid}")]
        public async Task<IActionResult> UpdateUserQuiz(string quizid, [FromBody] RegisterQuizRequest registerQuizRequest)
        {
            var result = await _registerCourseService.UpdateUserQuiz(quizid, registerQuizRequest);
            return Ok(result);
        }

        [HttpGet("get-enroll-chapters-by/{enrollCourseId}")]
        public async Task<IActionResult> GetEnrollChaptersByEnrollCourseId(string enrollCourseId)
        {
            var result = await _registerCourseService.GetEnrollChaptersByEnrollCourseId(enrollCourseId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollCourseById([FromRoute]string id)
        {
            var res = await _registerCourseService.GetEnrollCourseById(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-certificate-by/{certificateId}")]
        public async Task<IActionResult> GetCertificateById([FromRoute]string certificateId)
        {
            var res = await _registerCourseService.GetCertificateById(certificateId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get-all-certificates")]
        public async Task<IActionResult> GetAllCertificates()
        {
            var res = await _registerCourseService.GetAllCertificates();
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get-certificates-by/{courseId}")]
        public async Task<IActionResult> GetCertificatesByCourseId([FromRoute] string courseId)
        {
            var res = await _registerCourseService.GetCertificatesByCourseId(courseId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get-certificate-by-customer")]
        public async Task<IActionResult> GetCertificateByCustomerId()
        {
            var res = await _registerCourseService.GetCertificateByCustomerId();
            return StatusCode(res.StatusCode, res);
        }
        [HttpPost("create-certificate")]
        public async Task<IActionResult> CreateCertificate([FromForm] CertificateRequest certificateRequest)
        {
            var res = await _registerCourseService.CreateCertificate(certificateRequest);
            return StatusCode(res.StatusCode, res);
        }
    }
}
