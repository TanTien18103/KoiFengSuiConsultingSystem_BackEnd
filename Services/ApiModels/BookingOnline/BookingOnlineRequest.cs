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
        [Required]
        public string? MasterId { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}0-9 ]+$", ErrorMessage = "Mô tả không được chứa ký tự đặc biệt")]
        public string Description { get; set; }
        [Required]
        public DateOnly? BookingDate { get; set; }
        [Required]
        public TimeOnly? StartTime { get; set; }
        [Required]
        public TimeOnly? EndTime { get; set; }
    }
}
