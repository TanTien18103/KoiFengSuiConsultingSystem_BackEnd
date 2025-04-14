using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.KoiVariety
{
    public class ColorRequest
    {
        [Required(ErrorMessage = "Tên màu không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên màu không được vượt quá 100 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9 ]+$", ErrorMessage = "Tên màu không được chứa ký tự đặc biệt")]
        public string ColorName { get; set; }

        [Required(ErrorMessage = "Mã màu không được để trống.")]
        [RegularExpression(@"^[A-Fa-f0-9]{6}$", ErrorMessage = "Mã màu phải có định dạng hex gồm 6 ký tự (ví dụ: #FFFFFF).")]
        public string ColorCode { get; set; }

        [Required(ErrorMessage = "Mệnh không được để trống.")]
        [StringLength(100, ErrorMessage = "Mệnh không được vượt quá 100 ký tự.")]
        [RegularExpression(@"^[\p{L}0-9 ]+$", ErrorMessage = "Mệnh không được chứa ký tự đặc biệt")]
        public string Element { get; set; }
    }
}
