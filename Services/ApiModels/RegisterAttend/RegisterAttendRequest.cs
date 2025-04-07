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
        [Required]
        public string WorkshopId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng vé phải từ 1 trở lên.")]
        public int NumberOfTicket { get; set; }
    }
}
