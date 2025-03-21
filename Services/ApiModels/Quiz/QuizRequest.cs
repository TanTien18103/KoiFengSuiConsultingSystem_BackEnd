using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Quiz
{
    public class QuizRequest
    {
        public string Title { get; set; }
        public decimal? Score { get; set; }
        
    }
}
