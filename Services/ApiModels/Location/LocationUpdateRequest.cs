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

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên vị trí phải từ 2 đến 100 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.\-/]*$", ErrorMessage = "Tên vị trí không được chứa ký tự đặc biệt.")]
        public string? LocationName { get; set; }
    }
}
