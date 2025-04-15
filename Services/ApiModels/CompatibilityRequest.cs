using System.ComponentModel.DataAnnotations;

namespace Services.ApiModels
{
    public class CompatibilityRequest
    {
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Tỷ lệ màu cá là bắt buộc")]
        public Dictionary<string, double> ColorRatios { get; set; }

        [Required(ErrorMessage = "Hình dạng ao là bắt buộc")]
        [StringLength(100, ErrorMessage = "Hình dạng ao không được vượt quá 100 ký tự")]
        //[RegularExpression(@"^[a-zA-Z0-9\s\-_]+$", ErrorMessage = "Hình dạng ao chỉ được chứa chữ cái, số, dấu cách, dấu gạch ngang hoặc dấu gạch dưới")]
        public string PondShape { get; set; }

        [Required(ErrorMessage = "Hướng ao là bắt buộc")]
        [StringLength(100, ErrorMessage = "Hướng ao không được vượt quá 100 ký tự")]
        //[RegularExpression(@"^[a-zA-Z0-9\s\-_]+$", ErrorMessage = "Hướng ao chỉ được chứa chữ cái, số, dấu cách, dấu gạch ngang hoặc dấu gạch dưới")]
        public string PondDirection { get; set; }

        [Required(ErrorMessage = "Số lượng cá là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng cá phải là số nguyên dương")]
        public int FishCount { get; set; }
    }
}
