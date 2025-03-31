using Services.ApiModels.Attachment;
using Services.ApiModels;
﻿using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AttachmentService
{
    public interface IAttachmentService
    {
        Task<ResultModel> CreateAttachment(AttachmentRequest request);
        Task<ResultModel> GetAttachmentById(string attachmentId);
        Task<ResultModel> GetAttachmentByBookingOfflineId(string bookingOfflineId);
        Task<ResultModel> CancelAttachment(string attachmentId);
        Task<ResultModel> ConfirmAttachment(string attachmentId);
        Task<ResultModel> SendOtpForAttachment(string attachmentId);
        Task<ResultModel> VerifyAttachmentOtp(string attachmentId, VerifyOtpRequest request);
        Task<ResultModel> GetAllAttachments();
        Task<ResultModel> GetAllAttachmentsByMaster();
    }
}
