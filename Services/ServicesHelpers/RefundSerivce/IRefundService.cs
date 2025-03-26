using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers.RefundSerivce
{
    public interface IRefundService
    {
        Task<string> ProcessRefundAsync(RefundRequest request);
    }
}
