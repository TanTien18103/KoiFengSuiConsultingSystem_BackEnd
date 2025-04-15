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
        InProgress,
        ContractRejectedByManager,
        ContractConfirmedByManager,
        ContractRejectedByCustomer,
        ContractConfirmedByCustomer,
        VerifyingOTP,
        VerifiedOTP,
        FirstPaymentPending,
        FirstPaymentPendingConfirm,
        FirstPaymentSuccess,
        DocumentRejectedByManager,
        DocumentConfirmedByManager,
        DocumentRejectedByCustomer,
        DocumentConfirmedByCustomer,
        AttachmentRejected,
        AttachmentConfirmed,
        VerifyingOTPAttachment,
        VerifiedOTPAttachment,
        SecondPaymentPending,
        SecondPaymentPendingConfirm,
        Completed,
        Canceled,
    }
}
