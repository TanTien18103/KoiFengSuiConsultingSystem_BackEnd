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
        [RegularExpression(@"^[\p{L}0-9 ]+$", ErrorMessage = "Tên màu không được chứa ký tự đặc biệt")]
        public string ColorName { get; set; }

        [Required(ErrorMessage = "Mã màu không được để trống.")]
        public string ColorCode { get; set; }

        [Required(ErrorMessage = "Mệnh không được để trống.")]
        [RegularExpression(@"^[\p{L}0-9 ]+$", ErrorMessage = "Mệnh không được chứa ký tự đặc biệt")]
        public string Element { get; set; }
    }
}
