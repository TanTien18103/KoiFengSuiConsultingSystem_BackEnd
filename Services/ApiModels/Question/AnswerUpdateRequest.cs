using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Question
{
    public class AnswerUpdateRequest
    {
        public string AnswerId { get; set; }
        public string OptionText { get; set; }
        public string OptionType { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
