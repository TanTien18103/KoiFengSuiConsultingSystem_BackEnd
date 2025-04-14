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
        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn video để tải lên.")]
        public IFormFile Video { get; set; }
        [Required]
        public string CourseId { get; set; }
    }
}
