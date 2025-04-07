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
        [Required]
        public string ColorName { get; set; }
        [Required]
        public string ColorCode { get; set; }
        [Required]
        public string Element { get; set; }
    }
}
