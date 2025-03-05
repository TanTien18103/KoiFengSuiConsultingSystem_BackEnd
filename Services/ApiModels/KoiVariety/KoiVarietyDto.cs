using System;
using System.Collections.Generic;

namespace Services.ApiModels.KoiVariety
{
    public class KoiVarietyDto
    {
        public string KoiVarietyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool? IsActive { get; set; }
    }
} 