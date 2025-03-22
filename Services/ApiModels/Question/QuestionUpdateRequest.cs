using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Question
{
    public class QuestionUpdateRequest
    {
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public decimal? Point { get; set; }
    }
}
