using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AttachmentService
{
    public interface IAttachmentService
    {
        Task<ResultModel> GetAttachmentById(string id);
        Task<ResultModel> GetAllAttachments();
    }
}
