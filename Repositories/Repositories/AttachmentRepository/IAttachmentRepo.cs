using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.AttachmentRepository
{
    public interface IAttachmentRepo
    {
        Task<Attachment> GetAttachmentById(string attachmentId);
        Task<List<Attachment>> GetAttachments();
        Task<Attachment> CreateAttachment(Attachment attachment);
        Task<Attachment> UpdateAttachment(Attachment attachment);
        Task DeleteAttachment(string attachmentId);
        Task<List<Attachment>> GetAttachmentsByMaster(string masterId);
        Task<Attachment> UpdateAttachmentStatus(string attachmentId, string status);
        Task<Attachment> UpdateAttachmentWithBooking(string attachmentId, string bookingOfflineId);
    }
}
