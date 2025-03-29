using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum BookingOfflineEnums
    {
        Pending ,
        Confirmed,
        Completed,
        Cancelled,
        Paid1st,
        Paid2nd
    }
}
