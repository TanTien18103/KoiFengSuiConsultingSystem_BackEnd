using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IAttachmentRepo
    {
        Task<Attachment> GetAttachmentById(string attachmentId);
        Task<Attachment> CreateAttachment(Attachment attachment);
        Task<Attachment> UpdateAttachment(Attachment attachment);
        Task DeleteAttachment(string attachmentId);
    }
}
