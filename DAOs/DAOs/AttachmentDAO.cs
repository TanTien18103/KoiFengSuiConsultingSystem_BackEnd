using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class AttachmentDAO
    {
        private static volatile AttachmentDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private AttachmentDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static AttachmentDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new AttachmentDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Attachment> GetAttachmentByIdDao(string attachmentId)
        {
            return await _context.Attachments
                .Include(x => x.BookingOfflines).ThenInclude(x => x.Master)
                .Include(x => x.BookingOfflines).ThenInclude(x => x.Customer).ThenInclude(x => x.AccountId)
                .FirstOrDefaultAsync(x => x.AttachmentId == attachmentId);
        }

        public async Task<List<Attachment>> GetAttachmentsDao()
        {
            return await _context.Attachments.ToListAsync();
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
