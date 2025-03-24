using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Course
{
    public class CourseResponse
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string Author { get; set; }
        public string MasterId { get; set; }
        public string CategoryName { get; set; }
        public decimal? Rating { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
    }
}
