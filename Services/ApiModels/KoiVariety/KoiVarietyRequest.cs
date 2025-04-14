using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.KoiVariety
{
    public class KoiVarietyRequest
    {
        [Required(ErrorMessage = "Tên giống không được để trống.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên giống phải từ 3 đến 100 ký tự.")]
        [RegularExpression(@"^[\p{L}\s\-']+$", ErrorMessage = "Tên giống chỉ được chứa chữ cái, khoảng trắng, dấu gạch nối và dấu nháy đơn.")]
        public string VarietyName { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Mô tả phải từ 10 đến 500 ký tự.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Giới thiệu không được để trống.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Giới thiệu phải từ 10 đến 500 ký tự.")]
        public string Introduction { get; set; }

        public IFormFile ImageUrl { get; set; }

        [Required(ErrorMessage = "Danh sách màu giống không được để trống.")]
        public string VarietyColorsJson { get; set; }

        public List<VarietyColorRequest> GetVarietyColors()
        {
            return JsonConvert.DeserializeObject<List<VarietyColorRequest>>(VarietyColorsJson);
        }
        public class VarietyColorRequest
        {
            public string ColorId { get; set; }
            public decimal? Percentage { get; set; }
        }

    }
}
