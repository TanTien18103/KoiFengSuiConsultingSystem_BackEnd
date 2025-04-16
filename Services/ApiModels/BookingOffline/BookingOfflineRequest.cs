using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.BookingOffline
{
    public class BookingOfflineRequest
    {
        [Required(ErrorMessage = "Mô tả không được để trống.")]
        [RegularExpression(@"^[\p{L}0-9\s,.-]+$", ErrorMessage = "Mô tả không được chứa ký tự đặc biệt")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Mô tả phải có ít nhất 1 ký tự và không vượt quá 500 ký tự.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Địa điểm không được để trống.")]
        [RegularExpression(@"^[\p{L}0-9\s,./-]+$", ErrorMessage = "Địa điểm chỉ được chứa chữ cái, số, dấu cách, dấu phẩy, dấu chấm, dấu gạch nối và dấu chéo.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Địa điểm phải có ít nhất 1 ký tự và không vượt quá 200 ký tự.")]
        public string Location { get; set; }
    }
}
