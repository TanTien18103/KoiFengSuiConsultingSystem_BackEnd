using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Course
{
    public class CourseUpdateRequest
    {
        public string? CourseName { get; set; }
        public string? CourseCategory { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
