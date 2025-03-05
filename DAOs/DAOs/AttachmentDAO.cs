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
        public static AttachmentDAO instance = null;
        private readonly KoiFishPondContext _context;

        public AttachmentDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static AttachmentDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AttachmentDAO();
                }
                return instance;
            }
        }

        public async Task<Attachment> GetAttachmentByIdDao(string attachmentId)
        {
            return await _context.Attachments.FindAsync(attachmentId);
        }
        public async Task<Attachment> CreateAttachmentDao(Attachment attachment)
        {
            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task<Attachment> UpdateAttachmentDao(Attachment attachment)
        {
            _context.Attachments.Update(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task DeleteAttachmentDao(string attachmentId)
        {
            var attachment = await GetAttachmentByIdDao(attachmentId);
            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }
    }
}
