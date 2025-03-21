using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Course;
using Services.Services.CourseService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("get-all-course")]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _courseService.GetCourses();
            return Ok(courses);
        }

        [HttpGet("get-course/{id}")]
        public async Task<IActionResult> GetCourse(string id)
        {
            var course = await _courseService.GetCourseById(id);
            return Ok(course);
        }

        [HttpPost("create-course")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseRequest courseRequest)
        {
            var result = await _courseService.CreateCourse(courseRequest);
            return Ok(result);
        }

        [HttpPut("update-course/{id}")]
        public async Task<IActionResult> UpdateCourse(string id, [FromBody] CourseRequest courseRequest)
        {
            var result = await _courseService.UpdateCourse(id, courseRequest);
            return Ok(result);
        }

        [HttpDelete("delete-course/{id}")]
        public async Task<IActionResult> DeleteCourse(string id)
        {
            var result = await _courseService.DeleteCourse(id);
            return Ok(result);
        }

        [Authorize(Roles = "Master")]
        [HttpGet("get-all-course-by-master")]
        public async Task<IActionResult> GetAllCoursesByMaster()
        {
            var res = await _courseService.GetCoursesByMaster();
            return StatusCode(res.StatusCode, res);
        }
        
}
}
