using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.EnrollChapter
{
    public class EnrollChapterResponse
    {
        public string EnrollChapterId { get; set; }
        public string ChapterId { get; set; }
        public string ChapterName { get; set; }
        public string Status { get; set; }
        public string CustomerId { get; set; }
        public string EnrollCourseId { get; set; }
    }
}
