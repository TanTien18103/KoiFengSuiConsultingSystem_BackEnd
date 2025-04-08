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
        [Required]
        [RegularExpression(@"^[\p{L}0-9 ,.\\-_]+$", ErrorMessage = "Tên không được chứa ký tự đặc biệt")]
        public string MasterName { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}0-9 ,.\\-_]+$", ErrorMessage = "danh xưng không được chứa ký tự đặc biệt")]
        public string Title { get; set; }
        [Required]
        public string ServiceType { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}0-9 ,.\\-_]+$", ErrorMessage = "chuyên môn không được chứa ký tự đặc biệt")]
        public string Expertise { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}0-9 ,.\\-_]+$", ErrorMessage = "kinh nghiệm không được chứa ký tự đặc biệt")]
        public string Experience { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}0-9 ,.\\-_]+$", ErrorMessage = "lý lịch không được chứa ký tự đặc biệt")]
        public string Biography { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
