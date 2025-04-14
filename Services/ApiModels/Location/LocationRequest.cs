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
        [RegularExpression(@"^[a-zA-Z0-9\s,.\-/]*$", ErrorMessage = "Tên vị trí không được chứa ký tự đặc biệt.")]
        public string LocationName { get; set; }
    }
}
