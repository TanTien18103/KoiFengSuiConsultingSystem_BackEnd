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
        [Required]
        public string QuestionText { get; set; }
        [Required]
        public string QuestionType { get; set; }
        public List<AnswerRequest> Answers { get; set; } = new List<AnswerRequest>();
    }
}
