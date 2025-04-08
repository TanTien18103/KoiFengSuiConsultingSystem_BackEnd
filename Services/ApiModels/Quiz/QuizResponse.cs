using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Quiz
{
    public class QuizResponse
    {
        public string QuizId { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public int QuestionCount { get; set; }

        public string MasterId { get; set; }
        public string MasterName { get; set; }
        public DateTime? CreateAt { get; set; }
        public decimal? Score { get; set; }
    }
}
