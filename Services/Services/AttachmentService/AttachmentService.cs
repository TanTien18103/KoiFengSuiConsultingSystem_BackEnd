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
using System.Security.Claims;
using Repositories.Repositories.MasterRepository;
using BusinessObjects.Models;
using System.Reflection.Metadata;
using Services.ApiModels.FengShuiDocument;
using System.Xml.Linq;

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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMasterRepo _masterRepo;


        public AttachmentService(
            IAttachmentRepo attachmentRepo,
            IBookingOfflineRepo bookingOfflineRepo,
            IEmailService emailService,
            IUploadService uploadService,
            IMapper mapper,
            ILogger<AttachmentService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMasterRepo masterRepo)
        {
            _attachmentRepo = attachmentRepo;
            _bookingOfflineRepo = bookingOfflineRepo;
            _emailService = emailService;
            _uploadService = uploadService;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _masterRepo = masterRepo;
        }

        public async Task<ResultModel> CreateAttachment(AttachmentRequest request)
        {
            var res = new ResultModel();
            try
            {
                var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var claims = identity.Claims;
                var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var masterId = await _masterRepo.GetMasterIdByAccountId(accountId);

                var bookingOffline = await _bookingOfflineRepo.GetBookingOfflineById(request.BookingOfflineId);
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }

                var blockedStatuses = new[]
                {
                    BookingOfflineEnums.DocumentConfirmedByCustomer.ToString(),
                    BookingOfflineEnums.AttachmentRejected.ToString()
                };

                if (!blockedStatuses.Contains(bookingOffline.Status))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể tạo biên bản cho buổi tư vấn ở trạng thái hiện tại";
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
                    AttachmentUrl = fileUrl,
                    CreateBy = masterId
                };

                // Tạo attachment trước
                var createdAttachment = await _attachmentRepo.CreateAttachment(attachment);

                // Cập nhật trạng thái booking: Nếu trạng thái là AttachmentRejected, đổi thành DocumentConfirmedByCustomer
                string updatedStatus = bookingOffline.Status;
                if (bookingOffline.Status == BookingOfflineEnums.AttachmentRejected.ToString())
                {
                    updatedStatus = BookingOfflineEnums.DocumentConfirmedByCustomer.ToString();
                }
                    
                // Update booking với RecordId và trạng thái mới
                var updatedBooking = await _bookingOfflineRepo.UpdateBookingOfflineAttachment(
                    bookingOffline.BookingOfflineId,
                    createdAttachment.AttachmentId,
                    updatedStatus);

                if (updatedBooking == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể gắn tệp đính kèm vào buổi tư vấn";
                    return res;
                }

                var updatedAttachment = await _attachmentRepo.UpdateAttachmentWithBooking(
                    createdAttachment.AttachmentId,
                    updatedBooking.BookingOfflineId);

                if (updatedAttachment == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể cập nhật tài liệu với thông tin booking";
                    return res;
                }

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

        public async Task<ResultModel> GetAllAttachments()
        {
            var res = new ResultModel();
            try
            {
                var attachments = await _attachmentRepo.GetAttachments();
                if (attachments == null)
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
                res.Data = _mapper.Map<List<AllAttachmentResponse>>(attachments);
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

        public async Task<ResultModel> GetAllAttachmentsByMaster()
        {
            var res = new ResultModel();
            try
            {
                var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var claims = identity.Claims;
                var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var masterId = await _masterRepo.GetMasterIdByAccountId(accountId);

                var attachments = await _attachmentRepo.GetAttachmentsByMaster(masterId);
                if (attachments == null)
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
                res.Data = _mapper.Map<List<AllAttachmentResponse>>(attachments);
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

                var updatedAttachment = await _attachmentRepo.UpdateAttachmentStatus(attachmentId, AttachmentStatusEnums.Cancelled.ToString());

                if (updatedAttachment == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể hủy biên bản nghiệm thu";
                    return res;
                }

                var updatedBooking = await _bookingOfflineRepo.UpdateBookingOfflineAttachment(
                    attachment.BookingOfflines.FirstOrDefault()?.BookingOfflineId,
                    null,
                    BookingOfflineEnums.AttachmentRejected.ToString());

                var response = _mapper.Map<AttachmentResponse>(updatedAttachment);

                // Thêm thông tin booking
                if (updatedAttachment.BookingOfflines.Any())
                {
                    var bookingInfo = updatedAttachment.BookingOfflines.First();
                    response.BookingOffline = new BookingOfflineInfoForAttachment
                    {
                        BookingOfflineId = bookingInfo.BookingOfflineId,
                        CustomerName = bookingInfo.Customer?.Account?.FullName ?? "Không có thông tin",
                        MasterName = bookingInfo.Master?.Account?.FullName ?? "Không có thông tin"
                    };
                }

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

                var updatedAttachment = await _attachmentRepo.UpdateAttachmentStatus(attachmentId, AttachmentStatusEnums.Confirmed.ToString());

                if (updatedAttachment == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể hủy biên bản nghiệm thu";
                    return res;
                }

                var response = _mapper.Map<AttachmentResponse>(updatedAttachment);

                // Thêm thông tin booking
                if (updatedAttachment.BookingOfflines.Any())
                {
                    var bookingInfo = updatedAttachment.BookingOfflines.First();
                    response.BookingOffline = new BookingOfflineInfoForAttachment
                    {
                        BookingOfflineId = bookingInfo.BookingOfflineId,
                        CustomerName = bookingInfo.Customer?.Account?.FullName ?? "Không có thông tin", 
                        MasterName = bookingInfo.Master?.Account?.FullName ?? "Không có thông tin"
                    };
                }

                var booking = updatedAttachment.BookingOfflines.FirstOrDefault();
                if (booking != null)
                {
                    booking.Status = BookingOfflineEnums.AttachmentConfirmed.ToString();
                    await _bookingOfflineRepo.UpdateBookingOffline(booking);
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsAttachment.CONFIRM_SUCCESS;
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

                if (attachment.Status != AttachmentStatusEnums.Confirmed.ToString())
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
                bookingOffline.Status = BookingOfflineEnums.VerifyingOTPAttachment.ToString();
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);

                // Generate OTP 6 số
                var random = new Random();
                string otp = random.Next(100000, 999999).ToString();
                attachment.OtpCode = otp;
                attachment.OtpExpiredTime = DateTime.Now.AddMinutes(5);
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

                if (attachment.Status != AttachmentStatusEnums.Confirmed.ToString())
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

                var bookingOffline = attachment.BookingOfflines.FirstOrDefault();
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }
                bookingOffline.Status = BookingOfflineEnums.VerifiedOTPAttachment.ToString();
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);

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