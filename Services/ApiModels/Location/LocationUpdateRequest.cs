using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Location
{
    public class LocationUpdateRequest
    {

        [RegularExpression(@"^[\p{L}0-9 ,.\\-_]+$", ErrorMessage = "vị trí không được chứa ký tự đặc biệt")]
        public string? LocationName { get; set; }
    }
}
