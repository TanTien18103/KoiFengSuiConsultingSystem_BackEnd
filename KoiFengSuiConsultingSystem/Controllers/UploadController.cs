using BusinessObjects.Constants;
using BusinessObjects.Exceptions;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.ServicesHelpers.UploadService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;
        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null)
                    return BadRequest(new { success = false, message = "Không có file nào được chọn" });

                string imageUrl = await _uploadService.UploadImageAsync(file);

                if (imageUrl == null)
                    return BadRequest(new { success = false, message = "Upload thất bại" });

                return Ok(new { success = true, url = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPost("upload-images")]
        public async Task<IActionResult> UploadMultipleImages(List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                    return BadRequest(new { success = false, message = "Không có file nào được chọn" });

                var uploadedUrls = new List<string>();

                foreach (var file in files)
                {
                    string imageUrl = await _uploadService.UploadImageAsync(file);
                    if (imageUrl != null)
                    {
                        uploadedUrls.Add(imageUrl);
                    }
                }

                if (uploadedUrls.Count == 0)
                    return BadRequest(new { success = false, message = "Upload thất bại" });

                return Ok(new { success = true, urls = uploadedUrls });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("upload-video")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            try
            {
                if (file == null)
                    return BadRequest(new { success = false, message = "Không có file nào được chọn" });

                string videoUrl = await _uploadService.UploadVideoAsync(file);

                if (videoUrl == null)
                    return BadRequest(new { success = false, message = "Upload thất bại" });

                return Ok(new { success = true, url = videoUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-pdf-url/{publicId}")]
        public IActionResult GetPdfUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return BadRequest(new { success = false, message = "Public ID is required" });

            string pdfUrl = _uploadService.GetPdfUrl(publicId);

            return Ok(new { success = true, url = pdfUrl });
        }


        [HttpPost("UploadExcelFile")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Master")]
        public async Task<ActionResult<List<Quiz>>> UploadExcelFile(IFormFile file, [FromForm] string courseId)
        {
            try
            {
                var results = await _uploadService.UploadExcelAsync(file, courseId);
                var response = results.Select(quiz => new
                {
                    quiz.QuizId,
                    quiz.Title,
                    quiz.CourseId,
                    quiz.CreateBy,
                    quiz.CreateAt,
                    Questions = quiz.Questions.Select(q => new
                    {
                        q.QuestionId,
                        q.QuestionText,
                        q.QuestionType,
                        q.Point,
                        q.CreateAt,
                        Answers = q.Answers.Select(a => new
                        {
                            a.AnswerId,
                            a.OptionText,
                            a.OptionType,
                            a.IsCorrect,
                            a.CreateAt
                        })
                    })
                });

                return Ok(response);
            }
            catch (AppException ex)
            {
                return StatusCode(ex.StatusCode, new { code = ex.Code, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { code = ResponseCodeConstants.FAILED, message = ex.Message });
            }
        }
    }
}
