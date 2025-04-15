using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels
{
    public class MeetRequest
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-_]+$", ErrorMessage = "Tiêu đề chỉ được chứa chữ cái, số, dấu cách, dấu gạch ngang hoặc dấu gạch dưới")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Thời gian bắt đầu là bắt buộc")]
        [DataType(DataType.DateTime, ErrorMessage = "Thời gian bắt đầu không hợp lệ")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Thời gian kết thúc là bắt buộc")]
        [DataType(DataType.DateTime, ErrorMessage = "Thời gian kết thúc không hợp lệ")]
        public DateTime EndTime { get; set; }
    }
}
