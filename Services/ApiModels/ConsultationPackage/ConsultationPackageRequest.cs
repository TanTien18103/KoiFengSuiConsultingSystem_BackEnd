using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.ConsultationPackage
{
    public class ConsultationPackageRequest
    {
        [Required(ErrorMessage = "Tên gói không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên gói không được vượt quá 100 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s-_]+$", ErrorMessage = "Tên gói chỉ được chứa chữ cái, số, dấu cách, dấu gạch nối và dấu gạch dưới.")]
        public string PackageName { get; set; }

        [Required(ErrorMessage = "Giá tối thiểu không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá tối thiểu phải là số dương.")]
        public decimal? MinPrice { get; set; }

        [Required(ErrorMessage = "Giá tối đa không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá tối đa phải là số dương.")]
        [Compare("MinPrice", ErrorMessage = "Giá tối đa phải lớn hơn hoặc bằng giá tối thiểu.")]
        public decimal? MaxPrice { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s,.-_]+$", ErrorMessage = "Mô tả chỉ được chứa chữ cái, số, dấu cách, dấu phẩy, dấu chấm, dấu gạch nối và dấu gạch dưới.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thông tin phù hợp.")]
        [StringLength(200, ErrorMessage = "Thông tin phù hợp không được vượt quá 200 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s-_]+$", ErrorMessage = "Thông tin phù hợp chỉ được chứa chữ cái, số, dấu cách, dấu gạch nối và dấu gạch dưới.")]
        public string SuitableFor { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thông tin yêu cầu.")]
        [StringLength(300, ErrorMessage = "Thông tin yêu cầu không được vượt quá 300 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s-_]+$", ErrorMessage = "Thông tin yêu cầu chỉ được chứa chữ cái, số, dấu cách, dấu gạch nối và dấu gạch dưới.")]
        public string RequiredInfo { get; set; }

        [Required(ErrorMessage = "Chi tiết giá cả không được để trống.")]
        [StringLength(500, ErrorMessage = "Chi tiết giá cả không được vượt quá 500 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9\s,.-_]+$", ErrorMessage = "Chi tiết giá cả chỉ được chứa chữ cái, số, dấu cách, dấu phẩy, dấu chấm, dấu gạch nối và dấu gạch dưới.")]
        public string PricingDetails { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
