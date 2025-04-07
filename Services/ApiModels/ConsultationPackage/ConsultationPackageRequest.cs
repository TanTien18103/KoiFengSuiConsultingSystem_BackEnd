using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.ConsultationPackage
{
    public class ConsultationPackageRequest
    {
        [Required]
        public string PackageName { get; set; }
        [Required]
        public decimal? MinPrice { get; set; }
        [Required]
        public decimal? MaxPrice { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string SuitableFor { get; set; }
        [Required]
        public string RequiredInfo { get; set; }
        [Required]
        public string PricingDetails { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
