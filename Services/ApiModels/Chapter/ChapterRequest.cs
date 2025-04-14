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
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s-_]+$", ErrorMessage = "Tiêu đề chỉ được chứa chữ cái, số, dấu cách, dấu gạch nối và dấu gạch dưới.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s,.-_]+$", ErrorMessage = "Mô tả chỉ được chứa chữ cái, số, dấu cách, dấu phẩy, dấu chấm, dấu gạch nối và dấu gạch dưới.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn video để tải lên.")]
        public IFormFile Video { get; set; }
        [Required]
        public string CourseId { get; set; }
    }
}
