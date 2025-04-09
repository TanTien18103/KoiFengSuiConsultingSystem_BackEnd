using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Course
{
    public class CourseDetailResponse
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string CategoryName { get; set; }
        public string MasterId { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Rating { get; set; }
        public int? EnrolledStudents { get; set; }
        public string EnrollCourseId { get; set; }
        public int? TotalChapters { get; set; }
        public int? TotalQuestions { get; set; }
        public TimeOnly? TotalDuration { get; set; }
    }
}
