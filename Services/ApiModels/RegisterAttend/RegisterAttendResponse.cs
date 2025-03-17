using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.RegisterAttend
{
    public class RegisterAttendResponse
    {
        public string AttendId { get; set; }

        public string WorkshopName { get; set; }


        public string CustomerName { get; set; }

        public string Status { get; set; }
    }
}
