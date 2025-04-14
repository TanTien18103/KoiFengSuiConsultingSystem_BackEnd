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
        [Required(ErrorMessage = "Tên khóa học không được để trống.")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Danh mục khóa học không được để trống.")]
        public string CourseCategory { get; set; }

        [Required(ErrorMessage = "Mô tả khóa học không được để trống.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Giá khóa học không được để trống.")]
        public decimal? Price { get; set; }
        public IFormFile ImageUrl { get; set; }

    }
}
