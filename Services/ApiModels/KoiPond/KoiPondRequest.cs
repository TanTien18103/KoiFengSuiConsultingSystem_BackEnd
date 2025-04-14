using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.KoiPond
{
    public class KoiPondRequest
    {
        [Required(ErrorMessage = "Mã hình dạng không được để trống.")]
        public string ShapeId { get; set; }

        [Required(ErrorMessage = "Tên ao không được để trống.")]
        public string PondName { get; set; }

        [Required(ErrorMessage = "Giới thiệu không được để trống.")]
        public string Introduction { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        public string Description { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
