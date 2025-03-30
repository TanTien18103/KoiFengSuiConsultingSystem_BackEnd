using AutoMapper;
using BusinessObjects.Constants;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.AttachmentRepository;
using Services.ApiModels;
using Services.ApiModels.Attachment;
using Services.ApiModels.BookingOffline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.Services.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepo _attachmentRepo;
        private readonly IMapper _mapper;
        public AttachmentService(IAttachmentRepo attachmentRepo, IMapper mapper)
        {
            _attachmentRepo = attachmentRepo;
            _mapper = mapper;
        }
        public async Task<ResultModel> GetAttachmentById(string id)
        {
            var res = new ResultModel();
            try
            {
                var attachment = await _attachmentRepo.GetAttachmentById(id);
                if (attachment != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<AttachmentDetailsResponse>(attachment);
                    res.Message = ResponseMessageConstrantsAttachment.ATTACHMENT_FOUND;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsAttachment.ATTACHMENT_NOT_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetAllAttachments()
        {
            var res = new ResultModel();
            try
            {
                var attachments = await _attachmentRepo.GetAttachments();
                if (attachments != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<List<AllAttachmentResponse>>(attachments);
                    res.Message = ResponseMessageConstrantsAttachment.ATTACHMENT_FOUND;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsAttachment.ATTACHMENT_NOT_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }
    }
}
