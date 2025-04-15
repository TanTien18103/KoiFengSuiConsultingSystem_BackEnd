using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Account
{
    public class EditProfileRequest
    {
        [StringLength(50, ErrorMessage = "Tên người dùng không được vượt quá 50 ký tự")]
        public string? UserName { get; set; }

        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Tên đầy đủ không được vượt quá 200 ký tự")]
        public string? FullName { get; set; }

        public DateOnly? Dob { get; set; }

        public bool? Gender { get; set; }

        public int? BankId { get; set; }

        public string? AccountNo { get; set; }

        public string? AccountName { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
