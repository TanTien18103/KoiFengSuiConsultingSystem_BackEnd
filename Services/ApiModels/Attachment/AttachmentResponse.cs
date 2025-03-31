using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Attachment
{
    public class AttachmentResponse
    {
        public string AttachmentId { get; set; }
        public string Status { get; set; }
        public string DocNo { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreateBy { get; set; }
        public BookingOfflineInfoForAttachment BookingOffline { get; set; }
    }

    public class BookingOfflineInfoForAttachment
    {
        public string BookingOfflineId { get; set; }
        public string CustomerName { get; set; }
        public string MasterName { get; set; }
    }
}