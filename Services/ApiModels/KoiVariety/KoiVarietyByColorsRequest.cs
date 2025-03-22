using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.KoiVariety
{
    public class KoiVarietyByColorsRequest
    {
        [Required(ErrorMessage = "Danh sách màu không được để trống")]
        public List<string> ColorIds { get; set; }
    }
}
