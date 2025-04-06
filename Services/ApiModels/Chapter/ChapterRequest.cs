using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Chapter
{
    public class ChapterRequest
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public TimeOnly? Duration { get; set; }

        public IFormFile Video { get; set; }

        public string CourseId { get; set; }
    }
}
