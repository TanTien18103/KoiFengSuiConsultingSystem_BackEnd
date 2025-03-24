using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.RegisterCourse
{
    public class QuizResultResponse
    {
        public string QuizId { get; set; }
        public string ParticipantId { get; set; }
        public decimal TotalScore { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        
    }

}
