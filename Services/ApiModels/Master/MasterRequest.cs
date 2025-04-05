using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Master
{
    public class MasterRequest
    {
        public string MasterName { get; set; }
        public string Title { get; set; }
        public string ServiceType { get; set; }
        public string Expertise { get; set; }
        public string Experience { get; set; }
        public string Biography { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
