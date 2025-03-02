using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DTOs
{
    public class FishesWithColorsDTO
    {
        public string Id { get; set; }
        public string VarietyName { get; set; }
        public List<ColorPercentageDto> Colors { get; set; } = new List<ColorPercentageDto>();
    }
}
