using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Question;
using Services.Services.QuestionService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetQuestionById(string questionId)
        {
            var result = await _questionService.GetQuestionById(questionId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestions()
        {
            var result = await _questionService.GetQuestions();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("quiz/{quizId}")]
        public async Task<IActionResult> GetQuestionsByQuizId(string quizId)
        {
            var result = await _questionService.GetQuestionsByQuizId(quizId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("quiz/{quizId}")]
        public async Task<IActionResult> CreateQuestion(string quizId, [FromBody] QuestionRequest questionRequest)
        {
            var result = await _questionService.CreateQuestion(quizId, questionRequest);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{questionId}")]
        public async Task<IActionResult> UpdateQuestion(string questionId, [FromBody] QuestionRequest questionRequest)
        {
            var result = await _questionService.UpdateQuestion(questionId, questionRequest);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{questionId}")]
        public async Task<IActionResult> DeleteQuestion(string questionId)
        {
            var result = await _questionService.DeleteQuestion(questionId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
