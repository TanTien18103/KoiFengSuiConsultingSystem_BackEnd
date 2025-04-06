using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.RegisterCourse
{
    public class RegisterCourseResponse
    {
        public string EnrollCourseId { get; set; }

        public string CourseId { get; set; }

        public string EnrollCertId { get; set; }

        public string EnrollQuizId { get; set; }

        public decimal? Percentage { get; set; }

        public string Status { get; set; }

        public string CustomerId { get; set; }
    }
}
