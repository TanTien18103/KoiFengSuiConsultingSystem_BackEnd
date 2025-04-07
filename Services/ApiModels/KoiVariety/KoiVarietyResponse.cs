using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.KoiVariety
{
    public class KoiVarietyResponse
    {
        public string KoiVarietyId { get; set; }
        public string VarietyName { get; set; }
        public string Description { get; set; }
        public List<VarietyColorResponse> VarietyColors { get; set; }
        public decimal TotalPercentage { get; set; }
        public decimal CompatibilityScore { get; set; }
    }
    public class VarietyColorResponse
    {
        public decimal? Percentage { get; set; }
        public ColorResponse Color { get; set; }
    }
    public class ColorResponse
    {
        public string ColorId { get; set; }
        public string ColorName { get; set; }
        public string Element { get; set; }
    }
}
