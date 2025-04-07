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
        [Required]
        public string VarietyName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Introduction { get; set; }
        public IFormFile ImageUrl { get; set; }

        [Required]
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
