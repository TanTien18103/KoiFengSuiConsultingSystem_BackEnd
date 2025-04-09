using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.BookingOfflineRepository;
using Repositories.Repositories.CustomerRepository;
using Repositories.Repositories.FengShuiDocumentRepository;
using Repositories.Repositories.MasterRepository;
using Services.ApiModels;
using Services.ApiModels.FengShuiDocument;
using Services.ServicesHelpers.UploadService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;
using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.FengShuiDocument;

namespace Services.Services.FengShuiDocumentService
{
    public class FengShuiDocumentService : IFengShuiDocumentService
    {
        private readonly IFengShuiDocumentRepo _fengShuiDocumentRepo;
        private readonly IBookingOfflineRepo _bookingOfflineRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly IMasterRepo _masterRepo;
        private readonly IUploadService _uploadService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FengShuiDocumentService(
            IFengShuiDocumentRepo fengShuiDocumentRepo,
            IBookingOfflineRepo bookingOfflineRepo,
            ICustomerRepo customerRepo,
            IMasterRepo masterRepo,
            IMapper mapper,
            IUploadService uploadService,
            IHttpContextAccessor httpContextAccessor)
        {
            _fengShuiDocumentRepo = fengShuiDocumentRepo;
            _bookingOfflineRepo = bookingOfflineRepo;
            _customerRepo = customerRepo;
            _masterRepo = masterRepo;
            _mapper = mapper;
            _uploadService = uploadService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultModel> CreateFengShuiDocument(CreateFengShuiDocumentRequest request)
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
                    res.Message = "Không tìm thấy buổi tư vấn offline";
                    return res;
                }

                // Upload file PDF lên Cloudinary
                string pdfUrl = await _uploadService.UploadPdfAsync(request.PdfFile);
                if (string.IsNullOrEmpty(pdfUrl))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể upload file PDF";
                    return res;
                }

                // Tạo mới tài liệu phong thủy với trạng thái Pending
                var fengShuiDocument = new FengShuiDocument
                {
                    FengShuiDocumentId = Guid.NewGuid().ToString("N").Substring(0, 20),
                    Status = DocumentStatusEnum.Pending.ToString(),
                    Version = "1.0",
                    DocNo = $"FS_{DateTime.Now:yyyyMMddHHmmss}",
                    DocumentName = $"FengShui_{request.BookingOfflineId}_{DateTime.Now:yyyyMMdd}",
                    DocumentUrl = pdfUrl,
                    CreateDate = DateTime.Now,
                    CreateBy = masterId
                };

                var createdDocument = await _fengShuiDocumentRepo.CreateFengShuiDocument(fengShuiDocument);

                // Update booking với DocumentId
                bookingOffline.DocumentId = fengShuiDocument.FengShuiDocumentId;
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status201Created;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = "Tạo tài liệu Phong Thủy thành công";
                res.Data = new FengShuiDocumentResponse
                {
                    DocumentId = fengShuiDocument.FengShuiDocumentId,
                    Status = "Pending",
                    Version = "1.0",
                    DocNo = $"FS_{DateTime.Now:yyyyMMddHHmmss}",
                    DocumentName = fengShuiDocument.DocumentName,
                    DocumentUrl = pdfUrl,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreateBy = masterId,
                    BookingOffline = new BookingOfflineInfo
                    {
                        BookingOfflineId = bookingOffline.BookingOfflineId,
                        CustomerName = bookingOffline.Customer?.Account?.FullName ?? "Không có thông tin",
                        MasterName = bookingOffline.Master?.Account?.FullName ?? "Không có thông tin"
                    }
                };

                return res;
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : "Không có inner exception";

                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi tạo tài liệu Phong Thủy: {ex.Message}. Chi tiết: {innerExceptionMessage}";
                return res;
            }
        }

        public async Task<ResultModel> GetFengShuiDocumentByBookingOfflineId(string bookingOfflineId)
        {
            var res = new ResultModel();
            try
            {
                var document = await _fengShuiDocumentRepo.GetFengShuiDocumentByBookingOfflineId(bookingOfflineId);
                if (document == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = "Không tìm thấy tài liệu Phong Thủy cho buổi tư vấn này";
                    return res;
                }


                var response = _mapper.Map<FengShuiDocumentResponse>(document);

                // Thêm thông tin booking
                var booking = await _bookingOfflineRepo.GetBookingOfflineById(bookingOfflineId);
                if (booking != null)
                {
                    response.BookingOffline = new BookingOfflineInfo
                    {
                        BookingOfflineId = booking.BookingOfflineId,
                        CustomerName = booking.Customer?.Account?.FullName ?? "Không có thông tin",
                        MasterName = booking.Master?.Account?.FullName ?? "Không có thông tin"
                    };
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = "Lấy tài liệu Phong Thủy thành công";
                res.Data = response;

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy tài liệu Phong Thủy: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> CancelDocumentByManager(string documentId)
        {
            var res = new ResultModel();
            try
            {
                
                var document = await _fengShuiDocumentRepo.GetFengShuiDocumentById(documentId);
                if (document == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = "Không tìm thấy tài liệu Phong Thủy";
                    return res;
                }
                if (document.Status != DocumentStatusEnum.Pending.ToString() 
                   )
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể hủy tài liệu ở trạng thái hiện tại";
                    return res;
                }

                // Cập nhật trạng thái thành CancelledByManager
                var updatedDocument = await _fengShuiDocumentRepo.UpdateFengShuiDocumentStatus(documentId, DocumentStatusEnum.CancelledByManager.ToString());

                if (updatedDocument == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể hủy tài liệu Phong Thủy";
                    return res;
                }

                // Cập nhật booking để xóa DocumentId
                var booking = updatedDocument.BookingOfflines.FirstOrDefault();
                if (booking != null)
                {
                    booking.DocumentId = null;
                    booking.Status = BookingOfflineEnums.DocumentRejectedByManager.ToString();
                    await _bookingOfflineRepo.UpdateBookingOffline(booking);
                }

                var response = _mapper.Map<FengShuiDocumentResponse>(updatedDocument);

                // Thêm thông tin booking
                if (updatedDocument.BookingOfflines.Any())
                {
                    var bookingInfo = updatedDocument.BookingOfflines.First();
                    response.BookingOffline = new BookingOfflineInfo
                    {
                        BookingOfflineId = bookingInfo.BookingOfflineId,
                        CustomerName = bookingInfo.Customer?.Account?.FullName ?? "Không có thông tin",
                        MasterName = bookingInfo.Master?.Account?.FullName ?? "Không có thông tin"
                    };
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = "Hủy tài liệu Phong Thủy thành công";
                res.Data = response;

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi hủy tài liệu Phong Thủy: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> CancelDocumentByCustomer(string documentId)
        {
            var res = new ResultModel();
            try
            {
                
                var document = await _fengShuiDocumentRepo.GetFengShuiDocumentById(documentId);
                if (document == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = "Không tìm thấy tài liệu Phong Thủy";
                    return res;
                }

                // Kiểm tra trạng thái hiện tại
                if (document.Status != DocumentStatusEnum.Pending.ToString())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể hủy tài liệu ở trạng thái hiện tại";
                    return res;
                }

                
                var updatedDocument = await _fengShuiDocumentRepo.UpdateFengShuiDocumentStatus(documentId, DocumentStatusEnum.CancelledByCustomer.ToString());

                if (updatedDocument == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể hủy tài liệu Phong Thủy";
                    return res;
                }

                // Cập nhật booking để xóa DocumentId
                var updatedBooking = updatedDocument.BookingOfflines.FirstOrDefault();
                if (updatedBooking != null)
                {
                    updatedBooking.DocumentId = null;
                    updatedBooking.Status = BookingOfflineEnums.DocumentRejectedByCustomer.ToString();
                    await _bookingOfflineRepo.UpdateBookingOffline(updatedBooking);
                }

                var response = _mapper.Map<FengShuiDocumentResponse>(updatedDocument);

                // Thêm thông tin booking
                if (updatedDocument.BookingOfflines.Any())
                {
                    var bookingInfo = updatedDocument.BookingOfflines.First();
                    response.BookingOffline = new BookingOfflineInfo
                    {
                        BookingOfflineId = bookingInfo.BookingOfflineId,
                        CustomerName = bookingInfo.Customer?.Account?.FullName ?? "Không có thông tin",
                        MasterName = bookingInfo.Master?.Account?.FullName ?? "Không có thông tin"
                    };
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = "Hủy tài liệu Phong Thủy thành công";
                res.Data = response;

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi hủy tài liệu Phong Thủy: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> ConfirmDocumentByCustomer(string documentId)
        {
            var res = new ResultModel();
            try
            {
               
                var document = await _fengShuiDocumentRepo.GetFengShuiDocumentById(documentId);
                if (document == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = "Không tìm thấy tài liệu Phong Thủy";
                    return res;
                }

               
                if (document.Status != DocumentStatusEnum.ConfirmedByManager.ToString())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Tài liệu phải được Manager xác nhận trước khi khách hàng xác nhận";
                    return res;
                }

                
                var updatedDocument = await _fengShuiDocumentRepo.UpdateFengShuiDocumentStatus(documentId, DocumentStatusEnum.Success.ToString());

                if (updatedDocument == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể xác nhận tài liệu Phong Thủy";
                    return res;
                }

                var response = _mapper.Map<FengShuiDocumentResponse>(updatedDocument);

                // Thêm thông tin booking
                if (updatedDocument.BookingOfflines.Any())
                {
                    var bookingInfo = updatedDocument.BookingOfflines.First();
                    response.BookingOffline = new BookingOfflineInfo
                    {
                        BookingOfflineId = bookingInfo.BookingOfflineId,
                        CustomerName = bookingInfo.Customer?.Account?.FullName ?? "Không có thông tin",
                        MasterName = bookingInfo.Master?.Account?.FullName ?? "Không có thông tin"
                    };
                }

                var booking = updatedDocument.BookingOfflines.FirstOrDefault();
                if (booking != null)
                {
                    booking.DocumentId = null;
                    booking.Status = BookingOfflineEnums.DocumentConfirmedByCustomer.ToString();
                    await _bookingOfflineRepo.UpdateBookingOffline(booking);
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = "Xác nhận tài liệu Phong Thủy thành công";
                res.Data = response;

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi xác nhận tài liệu Phong Thủy: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> ConfirmDocumentByManager(string documentId)
        {
            var res = new ResultModel();
            try
            {
               
                var document = await _fengShuiDocumentRepo.GetFengShuiDocumentById(documentId);
                if (document == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = "Không tìm thấy tài liệu Phong Thủy";
                    return res;
                }

                // Kiểm tra trạng thái hiện tại
                if (document.Status != DocumentStatusEnum.Pending.ToString())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Tài liệu phải ở trạng thái InProgress để xác nhận";
                    return res;
                }

                // Cập nhật trạng thái thành ConfirmedByManager
                var updatedDocument = await _fengShuiDocumentRepo.UpdateFengShuiDocumentStatus(documentId, DocumentStatusEnum.ConfirmedByManager.ToString());

                if (updatedDocument == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể xác nhận tài liệu Phong Thủy";
                    return res;
                }

                var response = _mapper.Map<FengShuiDocumentResponse>(updatedDocument);

                // Thêm thông tin booking
                if (updatedDocument.BookingOfflines.Any())
                {
                    var bookingInfo = updatedDocument.BookingOfflines.First();
                    response.BookingOffline = new BookingOfflineInfo
                    {
                        BookingOfflineId = bookingInfo.BookingOfflineId,
                        CustomerName = bookingInfo.Customer?.Account?.FullName ?? "Không có thông tin",
                        MasterName = bookingInfo.Master?.Account?.FullName ?? "Không có thông tin"
                    };
                }

                var booking = updatedDocument.BookingOfflines.FirstOrDefault();
                if (booking != null)
                {
                    booking.Status = BookingOfflineEnums.DocumentConfirmedByManager.ToString();
                    await _bookingOfflineRepo.UpdateBookingOffline(booking);
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = "Xác nhận tài liệu Phong Thủy thành công";
                res.Data = response;

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi xác nhận tài liệu Phong Thủy: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> GetFengShuiDocumentById(string id)
        {
            var res = new ResultModel();
            try
            {
                var fengShuiDocument = await _fengShuiDocumentRepo.GetFengShuiDocumentById(id);
                if (fengShuiDocument != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<FengShuiDocumentResponse>(fengShuiDocument);
                    res.Message = ResponseMessageConstrantsFengShuiDocument.FENGSHUIDOCUMENT_FOUND;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsFengShuiDocument.FENGSHUIDOCUMENT_NOT_FOUND;
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

        public async Task<ResultModel> GetAllFengShuiDocuments()
        {
            var res = new ResultModel();
            try
            {
                var fengShuiDocuments = await _fengShuiDocumentRepo.GetFengShuiDocuments();
                if (fengShuiDocuments != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<List<AllFengShuiDocumentResponse>>(fengShuiDocuments);
                    res.Message = ResponseMessageConstrantsFengShuiDocument.FENGSHUIDOCUMENT_FOUND;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsFengShuiDocument.FENGSHUIDOCUMENT_NOT_FOUND;
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

        public async Task<ResultModel> GetAllFengShuiDocumentsByMaster()
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

                var fengShuiDocuments = await _fengShuiDocumentRepo.GetFengShuiDocumentsByMaster(masterId);
                if (fengShuiDocuments != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<List<AllFengShuiDocumentResponse>>(fengShuiDocuments);
                    res.Message = ResponseMessageConstrantsFengShuiDocument.FENGSHUIDOCUMENT_FOUND;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsFengShuiDocument.FENGSHUIDOCUMENT_NOT_FOUND;
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
