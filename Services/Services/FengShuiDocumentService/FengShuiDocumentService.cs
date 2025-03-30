using AutoMapper;
using BusinessObjects.Constants;
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

        private string GetAuthenticatedAccountId()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated) return null;

            var accountIdClaim = identity.FindFirst("AccountId");
            return accountIdClaim?.Value;
        }

        public async Task<ResultModel> CreateFengShuiDocument(CreateFengShuiDocumentRequest request)
        {
            var res = new ResultModel();
            try
            {
                // Kiểm tra booking offline tồn tại
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

                // Tạo mới tài liệu phong thủy
                var fengShuiDocument = new FengShuiDocument
                {
                    FengShuiDocumentId = Guid.NewGuid().ToString(),
                    Version = "1.0",  // Giá trị mặc định
                    Status = "Pending",
                    DocNo = $"FS_{DateTime.Now:yyyyMMddHHmmss}",
                    DocumentName = $"FengShui_{bookingOffline.BookingOfflineId}_{DateTime.Now:yyyyMMdd}",
                    DocumentUrl = pdfUrl
                };

                // Lưu tài liệu vào database
                var createdDocument = await _fengShuiDocumentRepo.CreateFengShuiDocument(fengShuiDocument);
                if (createdDocument == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể tạo tài liệu Phong Thủy";
                    return res;
                }

                // Cập nhật booking với DocumentId
                bookingOffline.DocumentId = createdDocument.FengShuiDocumentId;
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);

                // Lấy thông tin booking đã cập nhật
                var updatedBooking = await _bookingOfflineRepo.GetBookingOfflineById(bookingOffline.BookingOfflineId);

                // Tạo response
                var response = new FengShuiDocumentResponse
                {
                    FengShuiDocumentId = createdDocument.FengShuiDocumentId,
                    Version = createdDocument.Version,
                    Status = createdDocument.Status,
                    DocNo = createdDocument.DocNo,
                    DocumentName = createdDocument.DocumentName,
                    DocumentUrl = pdfUrl,
                    BookingOffline = new BookingOfflineInfo
                    {
                        BookingOfflineId = updatedBooking.BookingOfflineId,
                        CustomerName = updatedBooking.Customer?.Account?.FullName ?? "Không có thông tin",
                        MasterName = updatedBooking.Master?.Account?.FullName ?? "Không có thông tin"
                    }
                };

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status201Created;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = "Tạo tài liệu Phong Thủy thành công";
                res.Data = response;

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi tạo tài liệu Phong Thủy: {ex.Message}";
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
                // Kiểm tra người dùng hiện tại có phải là Manager không
                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = "Bạn chưa đăng nhập";
                    return res;
                }

                // Kiểm tra tài liệu tồn tại
                var document = await _fengShuiDocumentRepo.GetFengShuiDocumentById(documentId);
                if (document == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = "Không tìm thấy tài liệu Phong Thủy";
                    return res;
                }

                // Cập nhật trạng thái
                var updatedDocument = await _fengShuiDocumentRepo.UpdateFengShuiDocumentStatus(documentId, "CancelledByManager");

                if (updatedDocument == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể hủy tài liệu Phong Thủy";
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
                // Kiểm tra người dùng hiện tại có phải là Customer không
                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = "Bạn chưa đăng nhập";
                    return res;
                }

                var customer = await _customerRepo.GetCustomerByAccountId(accountId);
                if (customer == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                    res.Message = "Bạn không phải là khách hàng";
                    return res;
                }

                // Kiểm tra tài liệu tồn tại
                var document = await _fengShuiDocumentRepo.GetFengShuiDocumentById(documentId);
                if (document == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = "Không tìm thấy tài liệu Phong Thủy";
                    return res;
                }

                // Kiểm tra tài liệu thuộc về khách hàng này
                var booking = document.BookingOfflines.FirstOrDefault();
                if (booking == null || booking.CustomerId != customer.CustomerId)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                    res.Message = "Tài liệu này không thuộc về bạn";
                    return res;
                }

                // Cập nhật trạng thái
                var updatedDocument = await _fengShuiDocumentRepo.UpdateFengShuiDocumentStatus(documentId, "CancelledByCustomer");

                if (updatedDocument == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể hủy tài liệu Phong Thủy";
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
                // Kiểm tra người dùng hiện tại có phải là Customer không
                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = "Bạn chưa đăng nhập";
                    return res;
                }

                var customer = await _customerRepo.GetCustomerByAccountId(accountId);
                if (customer == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                    res.Message = "Bạn không phải là khách hàng";
                    return res;
                }

                // Kiểm tra tài liệu tồn tại
                var document = await _fengShuiDocumentRepo.GetFengShuiDocumentById(documentId);
                if (document == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = "Không tìm thấy tài liệu Phong Thủy";
                    return res;
                }

                // Kiểm tra tài liệu thuộc về khách hàng này
                var booking = document.BookingOfflines.FirstOrDefault();
                if (booking == null || booking.CustomerId != customer.CustomerId)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    res.ResponseCode = ResponseCodeConstants.FORBIDDEN;
                    res.Message = "Tài liệu này không thuộc về bạn";
                    return res;
                }

                // Cập nhật trạng thái
                var updatedDocument = await _fengShuiDocumentRepo.UpdateFengShuiDocumentStatus(documentId, "ConfirmedByCustomer");

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
                // Kiểm tra người dùng hiện tại có phải là Manager không
                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = "Bạn chưa đăng nhập";
                    return res;
                }

                // Kiểm tra tài liệu tồn tại
                var document = await _fengShuiDocumentRepo.GetFengShuiDocumentById(documentId);
                if (document == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = "Không tìm thấy tài liệu Phong Thủy";
                    return res;
                }

                // Cập nhật trạng thái
                var updatedDocument = await _fengShuiDocumentRepo.UpdateFengShuiDocumentStatus(documentId, "ConfirmedByManager");

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
    }
}
