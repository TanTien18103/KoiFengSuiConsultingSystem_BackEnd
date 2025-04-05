using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.ConsultationPackage
{
    public class ConsultationPackageRequest
    {
        public string PackageName { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string Description { get; set; }

        public string SuitableFor { get; set; }

        public string RequiredInfo { get; set; }

        public string PricingDetails { get; set; }

        public IFormFile ImageUrl { get; set; }
    }
}
