using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class AttachmentDAO
    {
        private readonly KoiFishPondContext _context;

        public AttachmentDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Attachment> GetAttachmentById(string attachmentId)
        {
            return await _context.Attachments.FindAsync(attachmentId);
        }
        public async Task<Attachment> CreateAttachment(Attachment attachment)
        {
            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task<Attachment> UpdateAttachment(Attachment attachment)
        {
            _context.Attachments.Update(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task DeleteAttachment(string attachmentId)
        {
            var attachment = await GetAttachmentById(attachmentId);
            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }
    }
}
