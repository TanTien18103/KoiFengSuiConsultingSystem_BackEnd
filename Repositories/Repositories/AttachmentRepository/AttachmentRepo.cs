using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.AttachmentRepository
{
    public class AttachmentRepo : IAttachmentRepo
    {
        public Task<Attachment> GetAttachmentById(string attachmentId)
        {
            return AttachmentDAO.Instance.GetAttachmentByIdDao(attachmentId);
        }
        public Task<List<Attachment>> GetAttachments()
        {
            return AttachmentDAO.Instance.GetAttachmentsDao();
        }
        public Task<List<Attachment>> GetAttachmentsByMaster(string masterId)
        {
            return AttachmentDAO.Instance.GetAttachmentsByMasterDao(masterId);
        }
        public Task<Attachment> CreateAttachment(Attachment attachment)
        {
            return AttachmentDAO.Instance.CreateAttachmentDao(attachment);
        }
        public Task<Attachment> UpdateAttachment(Attachment attachment)
        {
            return AttachmentDAO.Instance.UpdateAttachmentDao(attachment);
        }
        public Task DeleteAttachment(string attachmentId)
        {
            return AttachmentDAO.Instance.DeleteAttachmentDao(attachmentId);
        }
    }
}
