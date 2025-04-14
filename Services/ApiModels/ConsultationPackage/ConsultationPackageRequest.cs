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
        public string PackageName { get; set; }

        [Required(ErrorMessage = "Giá tối thiểu không được để trống.")]
        public decimal? MinPrice { get; set; }

        [Required(ErrorMessage = "Giá tối đa không được để trống.")]
        public decimal? MaxPrice { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thông tin phù hợp.")]
        public string SuitableFor { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thông tin yêu cầu.")]
        public string RequiredInfo { get; set; }

        [Required(ErrorMessage = "Chi tiết giá cả không được để trống.")]
        public string PricingDetails { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
