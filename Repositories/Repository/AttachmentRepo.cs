using BusinessObjects.Models;
using DAOs.DAOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class AttachmentRepo : IAttachmentRepo
    {
        private readonly AttachmentDAO _attachmentDAO;

        public AttachmentRepo(AttachmentDAO attachmentDAO)
        {
            _attachmentDAO = attachmentDAO;
        }

        public async Task<Attachment> GetAttachmentById(string attachmentId)
        {
            return await _attachmentDAO.GetAttachmentById(attachmentId);
        }
        public async Task<Attachment> CreateAttachment(Attachment attachment)
        {
            return await _attachmentDAO.CreateAttachment(attachment);
        }

        public async Task<Attachment> UpdateAttachment(Attachment attachment)
        {
            return await _attachmentDAO.UpdateAttachment(attachment);
        }

        public async Task DeleteAttachment(string attachmentId)
        {
            await _attachmentDAO.DeleteAttachment(attachmentId);
        }

       
    }
}
