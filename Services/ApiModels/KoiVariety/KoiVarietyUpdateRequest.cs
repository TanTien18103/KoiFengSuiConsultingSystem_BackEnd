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
    public class KoiVarietyUpdateRequest
    {
        public string? VarietyName { get; set; }
        public string? Description { get; set; }
        public string? Introduction { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string? VarietyColorsJson { get; set; }
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
