using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Course
{
    public class CourseRequest
    {
        public string CourseName { get; set; }
        public string CourseCategory { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }

    }
}
