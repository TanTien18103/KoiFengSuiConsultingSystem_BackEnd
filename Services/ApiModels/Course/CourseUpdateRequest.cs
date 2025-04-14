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
        [StringLength(100, ErrorMessage = "Tên khóa học không được vượt quá 100 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s-_]+$", ErrorMessage = "Tên khóa học chỉ được chứa chữ cái, số, dấu cách, dấu gạch nối và dấu gạch dưới.")]
        public string? CourseName { get; set; }

        [StringLength(100, ErrorMessage = "Danh mục khóa học không được vượt quá 100 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s-_]+$", ErrorMessage = "Danh mục khóa học chỉ được chứa chữ cái, số, dấu cách, dấu gạch nối và dấu gạch dưới.")]
        public string? CourseCategory { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả khóa học không được vượt quá 500 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s,.-_]+$", ErrorMessage = "Mô tả khóa học chỉ được chứa chữ cái, số, dấu cách, dấu phẩy, dấu chấm, dấu gạch nối và dấu gạch dưới.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá khóa học không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá khóa học phải là số dương.")]
        public decimal? Price { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
