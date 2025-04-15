using BusinessObjects.Enums;
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

        [HttpGet("best-seller")]
        public async Task<IActionResult> GetIsBestSellerCourses()
        {
            var res = await _courseService.GetIsBestSellerCourses();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("sort-by-rating")]
        public async Task<IActionResult> SortByRating()
        {
            var res = await _courseService.SortByRating();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-by-category/{id}")]
        public async Task<IActionResult> GetCoursesByCategoryId([FromRoute]string id)
        {
            var res = await _courseService.GetCoursesByCategoryId(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-details-for-mobile/{id}")]
        public async Task<IActionResult> GetCourseByIdForMobile([FromRoute]string id)
        {
            var res = await _courseService.GetCourseByIdForMobile(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-paid-courses")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetPurchasedCourses()
        {
            var res = await _courseService.GetPurchasedCourses();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("create-course")]
        public async Task<IActionResult> CreateCourse([FromForm] CourseRequest courseRequest)
        {
            var result = await _courseService.CreateCourse(courseRequest);
            return Ok(result);
        }

        [HttpPut("update-course/{id}")]
        public async Task<IActionResult> UpdateCourse(string id, [FromForm] CourseUpdateRequest courseRequest)
        {
            var result = await _courseService.UpdateCourse(id, courseRequest);
            return Ok(result);
        }
        [HttpPut("update-course-status/{id}")]
        public async Task<IActionResult> UpdateCourseStatus(string id, [FromQuery] CourseStatusEnum status)
        {
            var result = await _courseService.UpdateCourseStatus(id, status);
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
        
        [HttpPost("rate/{courseId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RateCourse(string courseId, [FromBody] RatingRequest ratingRequest)
        {
            var result = await _courseService.RateCourse(courseId, ratingRequest.Rating);
            return StatusCode(result.StatusCode, result);
        }
    }
}
