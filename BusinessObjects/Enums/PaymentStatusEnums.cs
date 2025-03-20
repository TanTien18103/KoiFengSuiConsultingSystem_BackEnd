using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum PaymentStatusEnums
    {
        Pending = 1,
        PendingConfirm,
        Paid,
        Canceled,
        Expired,
        Paid1st,
        Paid2nd
    }
}
