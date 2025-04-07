using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Chapter
{
    public class ChapterRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public TimeOnly? Duration { get; set; }
        [Required]
        public IFormFile Video { get; set; }
        [Required]
        public string CourseId { get; set; }
    }
}
