using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Question
{
    public class QuestionRequest
    {
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public decimal? Point { get; set; }
        public List<AnswerRequest> Answers { get; set; }
    }
    public class AnswerRequest
    {
        public string OptionText { get; set; }
        public string OptionType { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
