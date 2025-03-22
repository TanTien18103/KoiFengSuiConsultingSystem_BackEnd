using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Answer;
using Services.Services.AnswerService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService _answerService;

        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        [HttpPost("{questionId}")]
        public async Task<IActionResult> CreateAnswer(string questionId, [FromBody] AnswerRequest answerRequest)
        {
            var result = await _answerService.CreateAnswer(questionId, answerRequest);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{answerId}")]
        public async Task<IActionResult> DeleteAnswer(string answerId)
        {
            var result = await _answerService.DeleteAnswer(answerId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{answerId}")]
        public async Task<IActionResult> GetAnswerById(string answerId)
        {
            var result = await _answerService.GetAnswerById(answerId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{answerId}")]
        public async Task<IActionResult> UpdateAnswer(string answerId, [FromBody] AnswerRequest answerRequest)
        {
            var result = await _answerService.UpdateAnswer(answerId, answerRequest);
            return StatusCode(result.StatusCode, result);
        }
    }
}
