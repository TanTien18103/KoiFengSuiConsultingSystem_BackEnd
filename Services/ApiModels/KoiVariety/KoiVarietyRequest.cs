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
        public string VarietyName { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Giới thiệu không được để trống.")]
        public string Introduction { get; set; }

        [Required(ErrorMessage = "Ảnh giống không được để trống.")]
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
