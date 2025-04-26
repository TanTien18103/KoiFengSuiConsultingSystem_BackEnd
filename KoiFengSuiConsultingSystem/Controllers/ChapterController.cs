using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Chapter;
using Services.Services.ChapterService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly IChapterService _chapterService;

        public ChapterController(IChapterService chapterService)
        {
            _chapterService = chapterService;
        }

        [HttpGet("get-all-chapters-by-courseId")]
        public async Task<IActionResult> GetChapterByCouresId(string id)
        {
            var Chapter = await _chapterService.GetChaptersByCourseId(id);
            return Ok(Chapter);
        }

        [HttpGet("get-chapter/{id}")]
        public async Task<IActionResult> GetChapterById(string id)
        {
            var chapter = await _chapterService.GetChapterById(id);
            return Ok(chapter);
        }

        [HttpPost("create-chapter")]
        [RequestSizeLimit(500_000_000)] // 500MB
        [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
        public async Task<IActionResult> CreateChapter([FromForm] ChapterRequest request)
        {
            try
            {
                if (request.Video == null || request.Video.Length == 0)
                    return BadRequest(new { success = false, message = "Không có file nào được chọn" });

                string[] allowedTypes = { "video/mp4", "video/mpeg", "video/quicktime", "video/x-msvideo" };
                if (!allowedTypes.Contains(request.Video.ContentType))
                    return BadRequest(new { success = false, message = "Định dạng file không được hỗ trợ" });

                // Gọi trực tiếp dịch vụ tạo chapter với request hiện tại
                var result = await _chapterService.CreateChapter(request);

                if (result.IsSuccess)
                    return Ok(result);
                else
                    return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi tải lên video",
                    error = ex.Message
                });
            }
        }

        [HttpPut("update-chapter/{id}")]
        [RequestSizeLimit(500_000_000)] // 500MB
        [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
        public async Task<IActionResult> Updatechapter(string id, [FromForm] ChapterUpdateRequest chapterRequest)
        {
            try
            {
                if (chapterRequest.Video == null || chapterRequest.Video.Length == 0)
                    return BadRequest(new { success = false, message = "Không có file nào được chọn" });

                string[] allowedTypes = { "video/mp4", "video/mpeg", "video/quicktime", "video/x-msvideo" };
                if (!allowedTypes.Contains(chapterRequest.Video.ContentType))
                    return BadRequest(new { success = false, message = "Định dạng file không được hỗ trợ" });

                // Gọi trực tiếp dịch vụ tạo chapter với request hiện tại
                var result = await _chapterService.UpdateChapter(id, chapterRequest);

                if (result.IsSuccess)
                    return Ok(result);
                else
                    return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi tải lên video",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("delete-chapter/{id}")]
        public async Task<IActionResult> Deletechapter(string id)
        {
            var result = await _chapterService.DeleteChapter(id);
            return Ok(result);
        }
    }
}
