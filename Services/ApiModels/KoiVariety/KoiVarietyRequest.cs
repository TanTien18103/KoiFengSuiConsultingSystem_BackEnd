using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.KoiVariety
{
    public class KoiVarietyRequest
    {
        public string VarietyName { get; set; }
        public string Description { get; set; }
        public IFormFile ImageUrl { get; set; }
        public List<VarietyColorRequest> VarietyColors { get; set; } = new List<VarietyColorRequest>();

        public class VarietyColorRequest
        {
            public string ColorId { get; set; }
            public decimal? Percentage { get; set; }
        }

    }
}
