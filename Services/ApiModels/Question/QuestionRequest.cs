using Services.ApiModels.Answer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Question
{
    public class QuestionRequest
    {
        [Required(ErrorMessage = "Câu hỏi không được để trống")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Câu hỏi phải từ 5 đến 1000 ký tự")]
        public string QuestionText { get; set; }

        public string QuestionType { get; set; }

        public List<AnswerRequest> Answers { get; set; } = new List<AnswerRequest>();
    }
}
