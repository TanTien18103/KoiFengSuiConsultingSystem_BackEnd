using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Location
{
    public class LocationRequest
    {
        [Required]
        public string LocationName { get; set; }
    }
}
