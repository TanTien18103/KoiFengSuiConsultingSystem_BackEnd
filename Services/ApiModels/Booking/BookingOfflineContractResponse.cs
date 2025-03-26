using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Booking
{
    public class BookingOfflineContractResponse
    {
        public string BookingOfflineId { get; set; }
        public string Status { get; set; }
        public decimal? SelectedPrice { get; set; }
        public string ContractId { get; set; }
        public virtual ContractUrlOnlyResponse Contract { get; set; }

    }

    public class ContractUrlOnlyResponse
    {
        public string ContractUrl { get; set; }
    }
}
