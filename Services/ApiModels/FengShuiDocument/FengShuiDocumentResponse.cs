using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.FengShuiDocument
{
    public class FengShuiDocumentResponse
    {
        public string FengShuiDocumentId { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        public string DocNo { get; set; }
        public string DocumentName { get; set; }
        public string DocumentUrl { get; set; }
        public BookingOfflineInfo BookingOffline { get; set; }
    }

    public class BookingOfflineInfo
    {
        public string BookingOfflineId { get; set; }
        public string CustomerName { get; set; }
        public string MasterName { get; set; }
    }
}
