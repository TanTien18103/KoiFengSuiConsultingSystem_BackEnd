using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Quiz;
using Services.Services.QuizService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizzes()
        {
            var result = await _quizService.GetQuizzes();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("by-master")]
        public async Task<IActionResult> GetQuizzesByMaster()
        {
            var result = await _quizService.GetQuizzesByMaster();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuizById(string quizId)
        {
            var result = await _quizService.GetQuizById(quizId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{courseId}")]
        public async Task<IActionResult> CreateQuiz(string courseId, [FromBody] QuizRequest quizRequest)
        {
            var result = await _quizService.CreateQuiz(courseId, quizRequest);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{courseId}")]
        public async Task<IActionResult> UpdateQuiz(string courseId, [FromBody] QuizRequest quizRequest)
        {
            var result = await _quizService.UpdateQuiz(courseId, quizRequest);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuiz(string quizId)
        {
            var result = await _quizService.DeleteQuiz(quizId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
