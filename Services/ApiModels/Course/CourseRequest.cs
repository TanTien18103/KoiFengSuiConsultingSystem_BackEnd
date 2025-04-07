using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Course
{
    public class CourseRequest
    {
        [Required]
        public string CourseName { get; set; }
        [Required]
        public string CourseCategory { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public IFormFile ImageUrl { get; set; }

    }
}
