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
        public async Task<IActionResult> Createchapter([FromForm] ChapterRequest chapterRequest)
        {
            var result = await _chapterService.CreateChapter(chapterRequest);
            return Ok(result);
        }

        [HttpPut("update-chapter/{id}")]
        public async Task<IActionResult> Updatechapter(string id, [FromForm] ChapterRequest chapterRequest)
        {
            var result = await _chapterService.UpdateChapter(id, chapterRequest);
            return Ok(result);
        }

        [HttpDelete("delete-chapter/{id}")]
        public async Task<IActionResult> Deletechapter(string id)
        {
            var result = await _chapterService.DeleteChapter(id);
            return Ok(result);
        }
    }
}
