using Services.ApiModels.Answer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Question
{
    public class QuestionUpdateRequest
    {
        [RegularExpression(@"^[\p{L}0-9,.\-_/ ]+$", ErrorMessage = "QuestionText không được chứa ký tự đặc biệt")]
        public string? QuestionText { get; set; }

        [RegularExpression(@"^[\p{L}0-9,.\-_/ ]+$", ErrorMessage = "QuestionType không được chứa ký tự đặc biệt")]
        public string? QuestionType { get; set; }

        public List<AnswerUpdateRequest> answerUpdateRequests { get; set; } = new List<AnswerUpdateRequest>();
    }
}
