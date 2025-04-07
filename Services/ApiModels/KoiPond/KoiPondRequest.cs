using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.KoiPond
{
    public class KoiPondRequest
    {
        [Required]
        public string ShapeId { get; set; }
        [Required]
        public string PondName { get; set; }
        [Required]
        public string Introduction { get; set; }
        [Required]
        public string Description { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
