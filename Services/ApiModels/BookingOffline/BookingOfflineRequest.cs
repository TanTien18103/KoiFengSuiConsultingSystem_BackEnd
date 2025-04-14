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
        [RegularExpression(@"^[\p{L}0-9 ]+$", ErrorMessage = "Mô tả không được chứa ký tự đặc biệt")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Địa điểm không được để trống.")]
        public string Location { get; set; }
    }
}
