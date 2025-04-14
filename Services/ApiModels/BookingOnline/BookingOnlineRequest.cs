using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.BookingOnline
{
    public class BookingOnlineRequest
    {
        public string? MasterId { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}0-9\s,.-]+$", ErrorMessage = "Mô tả không được chứa ký tự đặc biệt")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Mô tả phải có ít nhất 5 ký tự và không vượt quá 500 ký tự.")]
        public string Description { get; set; }

        [Required]
        public DateOnly BookingDate { get; set; }
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
    }
}
