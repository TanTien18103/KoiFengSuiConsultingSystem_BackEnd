using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum ContractStatusEnum
    {
        Pending,
        ContractRejectedByManager,
        ContractRejectedByCustomer,
        ContractApprovedByManager,
        VerifyingOTP,
        FirstPaymentPending,
        FirstPaymentSuccess
    }
}
