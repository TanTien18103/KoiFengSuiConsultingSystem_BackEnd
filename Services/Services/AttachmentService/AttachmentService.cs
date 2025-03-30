using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repositories.Repositories.AttachmentRepository;
using Repositories.Repositories.BookingOfflineRepository;
using Services.ApiModels.Attachment;
using Services.ApiModels;
using Services.Services.EmailService;
using Services.ServicesHelpers.UploadService;
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
        private readonly IBookingOfflineRepo _bookingOfflineRepo;
        private readonly IEmailService _emailService;
        private readonly IUploadService _uploadService;
        private readonly IMapper _mapper;
        private readonly ILogger<AttachmentService> _logger;

        public AttachmentService(
            IAttachmentRepo attachmentRepo,
            IBookingOfflineRepo bookingOfflineRepo,
            IEmailService emailService,
            IUploadService uploadService,
            IMapper mapper,
            ILogger<AttachmentService> logger)
        {
            _attachmentRepo = attachmentRepo;
            _bookingOfflineRepo = bookingOfflineRepo;
            _emailService = emailService;
            _uploadService = uploadService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultModel> CreateAttachment(AttachmentRequest request)
        {
            var res = new ResultModel();
            try
            {
                var bookingOffline = await _bookingOfflineRepo.GetBookingOfflineById(request.BookingOfflineId);
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }

                // Upload file PDF
                string fileUrl = await _uploadService.UploadPdfAsync(request.PdfFile);
                if (string.IsNullOrEmpty(fileUrl))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể upload file PDF";
                    return res;
                }

                var attachment = new BusinessObjects.Models.Attachment
                {
                    AttachmentId = Guid.NewGuid().ToString("N").Substring(0, 20),
                    Status = AttachmentStatusEnums.Pending.ToString(),
                    AttachmentName = $"Attachment_{request.BookingOfflineId}_{DateTime.Now:yyyyMMdd}",
                    DocNo = $"DOC_{DateTime.Now:yyyyMMddHHmmss}",
                    CreatedDate = DateTime.Now,
                    AttachmentUrl = fileUrl
                };

                var createdAttachment = await _attachmentRepo.CreateAttachment(attachment);

                // Update booking với RecordId
                bookingOffline.RecordId = attachment.AttachmentId;
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status201Created;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsAttachment.CREATED_SUCCESS;
                res.Data = _mapper.Map<AttachmentResponse>(createdAttachment);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo tệp đính kèm");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetAttachmentById(string attachmentId)
        {
            var res = new ResultModel();
            try
            {
                var attachment = await _attachmentRepo.GetAttachmentById(attachmentId);
                if (attachment == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsAttachment.NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsAttachment.FOUND;
                res.Data = _mapper.Map<AttachmentResponse>(attachment);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin tệp đính kèm");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetAttachmentByBookingOfflineId(string bookingOfflineId)
        {
            var res = new ResultModel();
            try
            {
                var bookingOffline = await _bookingOfflineRepo.GetBookingOfflineById(bookingOfflineId);
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }

                if (bookingOffline.RecordId == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsAttachment.NOT_FOUND;
                    return res;
                }

                var attachment = await _attachmentRepo.GetAttachmentById(bookingOffline.RecordId);
                if (attachment == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsAttachment.NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsAttachment.FOUND;
                res.Data = _mapper.Map<AttachmentResponse>(attachment);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tệp đính kèm theo booking");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> CancelAttachment(string attachmentId)
        {
            var res = new ResultModel();
            try
            {
                var attachment = await _attachmentRepo.GetAttachmentById(attachmentId);
                if (attachment == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsAttachment.NOT_FOUND;
                    return res;
                }

                // Tìm booking liên quan
                var bookingOffline = attachment.BookingOfflines.FirstOrDefault();
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }

                attachment.Status = AttachmentStatusEnums.Cancelled.ToString();
                attachment.UpdatedDate = DateTime.Now;

                // Xóa liên kết với booking
                bookingOffline.RecordId = null;

                var updatedAttachment = await _attachmentRepo.UpdateAttachment(attachment);
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsAttachment.CANCEL_SUCCESS;
                res.Data = _mapper.Map<AttachmentResponse>(updatedAttachment);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hủy tệp đính kèm");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> ConfirmAttachment(string attachmentId)
        {
            var res = new ResultModel();
            try
            {
                var attachment = await _attachmentRepo.GetAttachmentById(attachmentId);
                if (attachment == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsAttachment.NOT_FOUND;
                    return res;
                }

                if (attachment.Status != AttachmentStatusEnums.Pending.ToString())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsAttachment.CHECK_STATUS;
                    return res;
                }

                attachment.Status = AttachmentStatusEnums.VefifyingOTP.ToString();
                attachment.UpdatedDate = DateTime.Now;

                var updatedAttachment = await _attachmentRepo.UpdateAttachment(attachment);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsAttachment.CONFIRM_SUCCESS;
                res.Data = _mapper.Map<AttachmentResponse>(updatedAttachment);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác nhận tệp đính kèm");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> SendOtpForAttachment(string attachmentId)
        {
            var res = new ResultModel();
            try
            {
                var attachment = await _attachmentRepo.GetAttachmentById(attachmentId);
                if (attachment == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsAttachment.NOT_FOUND;
                    return res;
                }

                if (attachment.Status != AttachmentStatusEnums.VefifyingOTP.ToString())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsAttachment.CHECK_STATUS;
                    return res;
                }

                var bookingOffline = attachment.BookingOfflines.FirstOrDefault();
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }

                // Generate OTP 6 số
                var random = new Random();
                string otp = random.Next(100000, 999999).ToString();
                attachment.OtpCode = otp;
                attachment.OtpExpiredTime = DateTime.Now.AddMinutes(1);
                attachment.UpdatedDate = DateTime.Now;

                // Gửi OTP qua email
                var emailContent = $@"
                    <h2>Xác nhận tệp đính kèm</h2>
                    <p>Mã OTP của bạn là: <strong>{otp}</strong></p>
                    <p>Mã có hiệu lực trong vòng 60 giây.</p>
                    <p>Vui lòng không chia sẻ mã này với bất kỳ ai.</p>";

                await _emailService.SendEmail(
                    bookingOffline.Customer.Account.Email,
                    "Mã OTP xác nhận tệp đính kèm",
                    emailContent
                );

                await _attachmentRepo.UpdateAttachment(attachment);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsAttachment.SEND_OTP_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi OTP");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> VerifyAttachmentOtp(string attachmentId, VerifyOtpRequest request)
        {
            var res = new ResultModel();
            try
            {
                var attachment = await _attachmentRepo.GetAttachmentById(attachmentId);
                if (attachment == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsAttachment.NOT_FOUND;
                    return res;
                }

                if (attachment.Status != AttachmentStatusEnums.VefifyingOTP.ToString())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsAttachment.CHECK_STATUS;
                    return res;
                }

                if (attachment.OtpExpiredTime == null || DateTime.Now > attachment.OtpExpiredTime)
                {
                    attachment.OtpCode = null;
                    attachment.OtpExpiredTime = null;
                    await _attachmentRepo.UpdateAttachment(attachment);

                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsAttachment.OTP_EXPIRED;
                    return res;
                }

                if (attachment.OtpCode != request.OtpCode)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsAttachment.VERIFY_OTP_FAILED;
                    return res;
                }

                attachment.OtpCode = null;
                attachment.OtpExpiredTime = null;
                attachment.Status = AttachmentStatusEnums.Success.ToString();
                attachment.UpdatedDate = DateTime.Now;

                await _attachmentRepo.UpdateAttachment(attachment);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsAttachment.VERIFY_OTP_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác thực OTP");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }
    }

}