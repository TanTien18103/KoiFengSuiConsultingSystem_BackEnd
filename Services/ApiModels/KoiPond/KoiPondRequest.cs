using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.KoiPond
{
    public class KoiPondRequest
    {
        [Required(ErrorMessage = "Mã hình dạng không được để trống.")]
        public string ShapeId { get; set; }

        [Required(ErrorMessage = "Tên ao không được để trống.")]
        [StringLength(500, ErrorMessage = "Tên ao không được vượt quá 500 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s-_]+$", ErrorMessage = "Tên ao chỉ được chứa chữ cái, số, dấu cách, dấu gạch nối và dấu gạch dưới.")]
        public string PondName { get; set; }

        [Required(ErrorMessage = "Giới thiệu không được để trống.")]
        [StringLength(500, ErrorMessage = "Giới thiệu không được vượt quá 500 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s,.-_]+$", ErrorMessage = "Giới thiệu chỉ được chứa chữ cái, số, dấu cách, dấu phẩy, dấu chấm, dấu gạch nối và dấu gạch dưới.")]
        public string Introduction { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s,.-_]+$", ErrorMessage = "Mô tả chỉ được chứa chữ cái, số, dấu cách, dấu phẩy, dấu chấm, dấu gạch nối và dấu gạch dưới.")]
        public string Description { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
