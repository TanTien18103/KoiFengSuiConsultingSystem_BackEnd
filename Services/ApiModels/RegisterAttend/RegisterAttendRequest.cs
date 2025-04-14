using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.RegisterAttend
{
    public class RegisterAttendRequest
    {
        [Required(ErrorMessage = "WorkshopId không được để trống.")]
        public string WorkshopId { get; set; }

        [Required(ErrorMessage = "Số lượng vé không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng vé phải từ 1 trở lên.")]
        public int NumberOfTicket { get; set; }

    }
}
