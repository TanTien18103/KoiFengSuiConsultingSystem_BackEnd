using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.KoiPond
{
    public class KoiPondRequest
    {
        public string ShapeId { get; set; }
        public string PondName { get; set; }
        public string Introduction { get; set; }
        public string Description { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
