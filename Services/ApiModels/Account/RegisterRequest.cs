using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Services.ApiModels.Account
{
    public class RegisterRequest
    {
        [Required]
        [RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Tên không được chứa số và các ký tự đặc biệt")]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 chữ số")]
        public string PhoneNumber { get; set; }

        [Required]
        public bool? Gender { get; set; }

        [Required]
        public DateTime Dob { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmedPassword { get; set; }

        public IFormFile ImageUrl { get; set; }

    }
}
