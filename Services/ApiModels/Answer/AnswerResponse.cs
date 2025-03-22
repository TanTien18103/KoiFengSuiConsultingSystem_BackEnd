using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Answer
{
    public class AnswerResponse
    {
        public string AnswerId { get; set; }

        public string QuestionId { get; set; }

        public string OptionText { get; set; }

        public string OptionType { get; set; }

        public DateTime? CreateAt { get; set; }

        public bool? IsCorrect { get; set; }
    }
}
