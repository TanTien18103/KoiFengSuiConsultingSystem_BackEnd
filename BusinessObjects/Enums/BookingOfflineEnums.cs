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
        [Display(Name = "Pending")]
        Pending = 0,

        [Display(Name = "Confirmed")]
        Confirmed = 1,

        [Display(Name = "Completed")]
        Completed = 2,

        [Display(Name = "Canceled")]
        Cancelled = 3,

        [Display(Name = "Paid1st")]
        Paid1st = 4,

        [Display(Name = "Paid2nd")]
        Paid2nd = 5
    }
}
