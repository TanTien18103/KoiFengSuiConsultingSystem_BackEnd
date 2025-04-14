using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Workshop
{
    public class WorkshopRequest
    {
        [Required(ErrorMessage = "Tên workshop là bắt buộc")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Tên workshop phải có từ 5 đến 100 ký tự")]
        [RegularExpression(@"^[\p{L}0-9 ,.\\-_]+$", ErrorMessage = "Tên workshop không được chứa ký tự đặc biệt")]
        public string WorkshopName { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Mã địa điểm là bắt buộc")]
        public string LocationId { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Mô tả phải có từ 10 đến 500 ký tự")]
        [RegularExpression(@"^[\p{L}0-9 ,.\\-_]+$", ErrorMessage = "Mô tả không được chứa ký tự đặc biệt")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Sức chứa là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải là số nguyên dương")]
        public int? Capacity { get; set; }

        [Required(ErrorMessage = "Giờ bắt đầu là bắt buộc")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "Giờ kết thúc là bắt buộc")]
        public string EndTime { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số dương")]
        public decimal? Price { get; set; }


        public IFormFile? ImageUrl { get; set; }

    }
}
