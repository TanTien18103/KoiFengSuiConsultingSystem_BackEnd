using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum BookingOnlineEnums
    {
        [Display(Name = "Pending")]
        Pending = 0,

        [Display(Name = "Confirmed")]
        Confirmed = 1,

        [Display(Name = "Completed")]
        Completed = 2,

        [Display(Name = "Canceled")]
        Cancelled = 3
    }
}
