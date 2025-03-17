using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.RegisterAttend
{
    public class RegisterAttendRequest
    {
        public string WorkshopId { get; set; }
        public int NumberOfTicket { get; set; }
    }
}
