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
        Pending,
        Confirmed,
        Cancelled,
        Approved,
        Verifying,
        Success,
        PendingPay1st,
        Paid1st,
        DocumentConfirmed,
        DocumentRejected,
        DocumentApproved,
        AttachmentSigning,
        AttachmentSigned,
        PendingPay2nd,
        Paid2nd,
    }
}
