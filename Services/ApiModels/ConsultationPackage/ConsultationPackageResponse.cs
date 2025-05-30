﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.ConsultationPackage
{
    public class ConsultationPackageResponse
    {
        public string ConsultationPackageId { get; set; }

        public string PackageName { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }
        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public string SuitableFor { get; set; }

        public string RequiredInfo { get; set; }

        public string PricingDetails { get; set; }
        public string Status { get; set; }
    }
}
