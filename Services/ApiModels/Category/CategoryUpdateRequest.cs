using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Category
{
    public class CategoryUpdateRequest
    {
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s-_]+$", ErrorMessage = "Tên danh mục chỉ được chứa chữ cái, số, dấu cách, dấu gạch nối và dấu gạch dưới.")]
        public string? CategoryName { get; set; }

        public IFormFile? ImageUrl { get; set; }
    }
}
