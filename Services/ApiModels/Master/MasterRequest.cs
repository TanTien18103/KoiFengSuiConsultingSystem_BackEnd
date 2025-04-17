using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Master
{
    public class MasterRequest
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên phải từ 3 đến 100 ký tự")]
        [RegularExpression(@"^[\p{L}0-9 ,.\-_]+$", ErrorMessage = "Tên không được chứa ký tự đặc biệt")]
        public string? MasterName { get; set; }

        [Required(ErrorMessage = "Danh xưng không được để trống")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Danh xưng phải từ 3 đến 100 ký tự")]
        [RegularExpression(@"^[\p{L}0-9 ,.\-_]+$", ErrorMessage = "Danh xưng không được chứa ký tự đặc biệt")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Loại dịch vụ không được để trống")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Loại dịch vụ phải từ 3 đến 500 ký tự")]
        public string ServiceType { get; set; }

        [Required(ErrorMessage = "Chuyên môn không được để trống")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Chuyên môn phải từ 10 đến 500 ký tự")]
        [RegularExpression(@"^[\p{L}0-9 ,.\-_]+$", ErrorMessage = "Chuyên môn không được chứa ký tự đặc biệt")]
        public string Expertise { get; set; }

        [Required(ErrorMessage = "Kinh nghiệm không được để trống")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Kinh nghiệm phải từ 10 đến 500 ký tự")]
        [RegularExpression(@"^[\p{L}0-9 ,.\-_]+$", ErrorMessage = "Kinh nghiệm không được chứa ký tự đặc biệt")]
        public string Experience { get; set; }

        [Required(ErrorMessage = "Lý lịch không được để trống")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Lý lịch phải từ 10 đến 1000 ký tự")]
        [RegularExpression(@"^[\p{L}0-9 ,.\-_]+$", ErrorMessage = "Lý lịch không được chứa ký tự đặc biệt")]
        public string Biography { get; set; }
        public string? LinkMeet { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
