using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Contract
{
    public class ContractResponse
    {
        public string ContractId { get; set; }
        public string DocNo { get; set; }
        public string Status { get; set; }
        public string ContractName { get; set; }
        public string ContractUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public BookingOfflineInfoForContract BookingOffline { get; set; }
    }

    public class BookingOfflineInfoForContract
    {
        public string BookingOfflineId { get; set; }
        public string CustomerName { get; set; }
        public string MasterName { get; set; }
    }
}
