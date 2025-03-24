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

        [HttpPost("create-by/{questionId}")]
        public async Task<IActionResult> CreateAnswer(string questionId, [FromBody] AnswerRequest answerRequest)
        {
            var result = await _answerService.CreateAnswer(questionId, answerRequest);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("delete-by/{answerId}")]
        public async Task<IActionResult> DeleteAnswer(string answerId)
        {
            var result = await _answerService.DeleteAnswer(answerId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-by/{answerId}")]
        public async Task<IActionResult> GetAnswerById(string answerId)
        {
            var result = await _answerService.GetAnswerById(answerId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update-by/{answerId}")]
        public async Task<IActionResult> UpdateAnswer(string answerId, [FromBody] AnswerRequest answerRequest)
        {
            var result = await _answerService.UpdateAnswer(answerId, answerRequest);
            return StatusCode(result.StatusCode, result);
        }
    }
}
