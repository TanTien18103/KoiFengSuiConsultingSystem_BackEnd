using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repositories.Repositories.BookingOfflineRepository;
using Repositories.Repositories.ContractRepository;
using Repositories.Repositories.OrderRepository;
using Services.ApiModels;
using Services.ApiModels.Contract;
using Services.ApiModels.FengShuiDocument;
using Services.Services.EmailService;
using Services.ServicesHelpers.PriceService;
using Services.ServicesHelpers.UploadService;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.Services.ContractService
{

    public class ContractService : IContractService
    {
        private readonly IContractRepo _contractRepo;
        private readonly IBookingOfflineRepo _bookingOfflineRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly IEmailService _emailService;
        private readonly IUploadService _uploadService;
        private readonly IPriceService _priceService;
        private readonly IMapper _mapper;
        private readonly ILogger<ContractService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContractService(
        IContractRepo contractRepo,
        IBookingOfflineRepo bookingOfflineRepo,
        IOrderRepo orderRepo,
        IEmailService emailService,
        IUploadService uploadService,
        IPriceService priceService,
        IMapper mapper,
        ILogger<ContractService> logger,
        IHttpContextAccessor httpContextAccessor)
        {
            _contractRepo = contractRepo;
            _bookingOfflineRepo = bookingOfflineRepo;
            _orderRepo = orderRepo;
            _emailService = emailService;
            _uploadService = uploadService;
            _priceService = priceService;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultModel> CancelContractByManager(string contractId)
        {
            var res = new ResultModel();
            try
            {
                var contract = await _contractRepo.GetContractById(contractId);
                if (contract == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsContract.NOT_FOUND;
                    return res;
                }
                var bookingOffline = contract.BookingOfflines.FirstOrDefault();
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }
                
                // Cập nhật trạng thái của bookingOffline và hủy liên kết với contract
                bookingOffline.Status = BookingOfflineEnums.ContractRejectedByManager.ToString();
                bookingOffline.ContractId = null;
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);
                
                // Cập nhật trạng thái của contract
                contract.Status = ContractStatusEnum.Cancelled.ToString();
                contract.UpdatedDate = DateTime.Now;
                var updatedContract = await _contractRepo.UpdateContract(contract);
                
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsContract.CANCEL_SUCCESS;
                res.Data = new ContractResponse
                {
                    ContractId = updatedContract.ContractId,
                    Status = updatedContract.Status,
                    DocNo = updatedContract.DocNo,
                    ContractName = updatedContract.ContractName,
                    ContractUrl = updatedContract.ContractUrl,
                    CreatedDate = updatedContract.CreatedDate,
                    UpdatedDate = updatedContract.UpdatedDate
                };
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hủy hợp đồng");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> ConfirmContractByManager(string contractId)
        {
            var res = new ResultModel();
            try
            {
                var contract = await _contractRepo.GetContractById(contractId);
                if (contract == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsContract.NOT_FOUND;
                    return res;
                }
                
                BookingOffline bookingOffline = null;
                
                // Kiểm tra xem contract có booking nào không
                if (contract.BookingOfflines == null || !contract.BookingOfflines.Any())
                {
                    // Nếu không có, tìm booking từ database dựa vào contractId
                    var bookings = await _bookingOfflineRepo.GetBookingOfflines();
                    bookingOffline = bookings.FirstOrDefault(b => b.ContractId == contractId);
                    
                    if (bookingOffline == null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                        return res;
                    }
                }
                else
                {
                    // Nếu có, lấy booking đầu tiên
                    bookingOffline = contract.BookingOfflines.FirstOrDefault();
                }
                
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }
                
                bookingOffline.Status = BookingOfflineEnums.ContractConfirmedByManager.ToString();
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);
                
                contract.Status = ContractStatusEnum.VerifyingOTP.ToString();
                contract.UpdatedDate = DateTime.Now;
                var updatedContract = await _contractRepo.UpdateContract(contract);
                
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsContract.CONFIRM_SUCCESS;
                res.Data = new ContractResponse
                {
                    ContractId = updatedContract.ContractId,
                    Status = updatedContract.Status,
                    DocNo = updatedContract.DocNo,
                    ContractName = updatedContract.ContractName,
                    ContractUrl = updatedContract.ContractUrl,
                    CreatedDate = updatedContract.CreatedDate,
                    UpdatedDate = updatedContract.UpdatedDate
                };
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác nhận hợp đồng");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> CancelContractByCustomer(string contractId)
        {
            var res = new ResultModel();
            try
            {
                var contract = await _contractRepo.GetContractById(contractId);
                if (contract == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsContract.NOT_FOUND;
                    return res;
                }
                var bookingOffline = contract.BookingOfflines.FirstOrDefault();
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }
                
                // Cập nhật trạng thái của bookingOffline và hủy liên kết với contract
                bookingOffline.Status = BookingOfflineEnums.ContractRejectedByCustomer.ToString();
                bookingOffline.ContractId = null;
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);
                
                // Cập nhật trạng thái của contract
                contract.Status = ContractStatusEnum.Cancelled.ToString();
                contract.UpdatedDate = DateTime.Now;
                var updatedContract = await _contractRepo.UpdateContract(contract);
                
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsContract.CANCEL_SUCCESS;
                res.Data = new ContractResponse
                {
                    ContractId = updatedContract.ContractId,
                    Status = updatedContract.Status,
                    DocNo = updatedContract.DocNo,
                    ContractName = updatedContract.ContractName,
                    ContractUrl = updatedContract.ContractUrl,
                    CreatedDate = updatedContract.CreatedDate,
                    UpdatedDate = updatedContract.UpdatedDate
                };
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hủy hợp đồng");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> ConfirmContractByCustomer(string contractId)
        {
            var res = new ResultModel();
            try
            {
                var contract = await _contractRepo.GetContractById(contractId);
                if (contract == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsContract.NOT_FOUND;
                    return res;
                }
                
                BookingOffline bookingOffline = null;
                
                // Kiểm tra xem contract có booking nào không
                if (contract.BookingOfflines == null || !contract.BookingOfflines.Any())
                {
                    // Nếu không có, tìm booking từ database dựa vào contractId
                    var bookings = await _bookingOfflineRepo.GetBookingOfflines();
                    bookingOffline = bookings.FirstOrDefault(b => b.ContractId == contractId);
                    
                    if (bookingOffline == null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                        return res;
                    }
                }
                else
                {
                    // Nếu có, lấy booking đầu tiên
                    bookingOffline = contract.BookingOfflines.FirstOrDefault();
                }
                
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }
                
                bookingOffline.Status = BookingOfflineEnums.ContractConfirmedByCustomer.ToString();
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);
                
                contract.Status = ContractStatusEnum.VerifyingOTP.ToString();
                contract.UpdatedDate = DateTime.Now;
                var updatedContract = await _contractRepo.UpdateContract(contract);
                
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsContract.CONFIRM_SUCCESS;
                res.Data = new ContractResponse
                {
                    ContractId = updatedContract.ContractId,
                    Status = updatedContract.Status,
                    DocNo = updatedContract.DocNo,
                    ContractName = updatedContract.ContractName,
                    ContractUrl = updatedContract.ContractUrl,
                    CreatedDate = updatedContract.CreatedDate,
                    UpdatedDate = updatedContract.UpdatedDate
                };
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác nhận hợp đồng");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetContractByBookingOfflineId(string bookingOfflineId)
        {
            var res = new ResultModel();
            try
            {
                var contract = await _contractRepo.GetContractByBookingOfflineId(bookingOfflineId);
                if (contract == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsContract.NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsContract.FOUND;
                res.Data = _mapper.Map<ContractResponse>(contract);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> CreateContract(ContractRequest request)
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

                var bookingOffline = await _bookingOfflineRepo.GetBookingOfflineById(request.BookingOfflineId);
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }

                // Upload file tạm thời
                string tempPdfUrl = await _uploadService.UploadPdfAsync(request.PdfFile);

                // Tạo đối tượng contract mới
                var contract = new BusinessObjects.Models.Contract
                {
                    ContractId = Guid.NewGuid().ToString("N").Substring(0, 20),
                    Status = ContractStatusEnum.Pending.ToString(),
                    ContractName = $"Contract_{request.BookingOfflineId}_{DateTime.Now:yyyyMMdd}",
                    DocNo = $"DOC_{DateTime.Now:yyyyMMddHHmmss}",
                    CreatedDate = DateTime.Now,
                    ContractUrl = tempPdfUrl,
                    CreateBy = accountId
                };

                // Tạo contract trong database
                var createdContract = await _contractRepo.CreateContract(contract);

                // Update booking với ContractId
                var updatedBooking = await _bookingOfflineRepo.UpdateBookingOfflineContract(
                    bookingOffline.BookingOfflineId,
                    createdContract.ContractId,
                    bookingOffline.Status); // Giữ nguyên trạng thái hiện tại

                if (updatedBooking == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = "Không thể gắn hợp đồng vào buổi tư vấn";
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status201Created;
                res.Message = ResponseMessageConstrantsContract.CREATED_SUCCESS;
                res.Data = new ContractResponse
                {
                    ContractId = $"C{DateTime.Now:yyMMddHHmmss}",
                    Status = ContractStatusEnum.Pending.ToString(),
                    DocNo = $"DOC_{DateTime.Now:yyyyMMddHHmmss}",
                    ContractName = contract.ContractName,
                    ContractUrl = tempPdfUrl,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreateBy = accountId
                };
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo hợp đồng");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetContractByBookingOfflineIdAndUpdateStatus(string bookingOfflineId)
        {
            var res = new ResultModel();
            try
            {
                var contract = await _contractRepo.GetContractByBookingOfflineId(bookingOfflineId);
                if (contract == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsContract.NOT_FOUND;
                    return res;
                }

                if (contract.Status != ContractStatusEnum.Pending.ToString())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsContract.CHECK_STATUS;
                    return res;
                }

                if (contract.Status == ContractStatusEnum.Pending.ToString())
                {
                    contract.Status = ContractStatusEnum.InProgress.ToString();
                    contract.UpdatedDate = DateTime.Now;
                    await _contractRepo.UpdateContract(contract);
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsContract.CONTRACT_INFORMATION_SUCCESS;
                res.Data = new
                {
                    Contract = new ContractResponse
                    {
                        ContractId = contract.ContractId,
                        Status = contract.Status,
                        DocNo = contract.DocNo,
                        ContractName = contract.ContractName,
                        ContractUrl = contract.ContractUrl,
                        CreatedDate = contract.CreatedDate,
                        UpdatedDate = contract.UpdatedDate
                    },
                };

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin hợp đồng");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        //public async Task<ResultModel> ProcessFirstPaymentAfterVerification(string contractId)
        //{
        //    var res = new ResultModel();
        //    try
        //    {
        //        var contract = await _contractRepo.GetContractById(contractId);
        //        if (contract == null)
        //        {
        //            res.IsSuccess = false;
        //            res.StatusCode = StatusCodes.Status404NotFound;
        //            res.Message = ResponseMessageConstrantsContract.NOT_FOUND;
        //            return res;
        //        }
        //        if (contract.Status != ContractStatusEnum.Success.ToString())
        //        {
        //            res.IsSuccess = false;
        //            res.StatusCode = StatusCodes.Status400BadRequest;
        //            res.Message = ResponseMessageConstrantsContract.CHECK_STATUS;
        //            return res;
        //        }
        //        var bookingOffline = contract.BookingOfflines.FirstOrDefault();
        //        if (bookingOffline == null)
        //        {
        //            res.IsSuccess = false;
        //            res.StatusCode = StatusCodes.Status404NotFound;
        //            res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
        //            return res;
        //        }
        //        if (bookingOffline.SelectedPrice == null)
        //        {
        //            res.IsSuccess = false;
        //            res.StatusCode = StatusCodes.Status400BadRequest;
        //            res.Message = ResponseMessageConstrantsBooking.NOT_SELECTED_PRICE_FOR_BOOKING;
        //            return res;           
        //          }
        //        decimal? firstPaymentAmount = await _priceService.GetServicePrice(PaymentTypeEnums.BookingOffline,bookingOffline.BookingOfflineId,true);

        //        if (firstPaymentAmount == null)
        //        {
        //            res.IsSuccess = false;
        //            res.StatusCode = StatusCodes.Status400BadRequest;
        //            res.Message = ResponseMessageConstrantsOrder.PRICE_NOT_FOUND_OR_INVALID;
        //            return res;
        //        }
        //        var order = new Order
        //        {
        //            OrderId = Guid.NewGuid().ToString("N").Substring(0, 20),
        //            CustomerId = bookingOffline.CustomerId,
        //            ServiceId = bookingOffline.BookingOfflineId,
        //            ServiceType = PaymentTypeEnums.BookingOffline.ToString(),
        //            Amount = firstPaymentAmount,
        //            OrderCode = $"ORD_{DateTime.Now:yyyyMMddHHmmss}",
        //            Status = PaymentStatusEnums.Pending.ToString(),
        //            CreatedDate = DateTime.Now,
        //            Description = $"Thanh toán lần 1 (30%) cho hợp đồng {contract.ContractName}",
        //            Note = $"ContractId: {contractId}",
        //            PaymentId = Guid.NewGuid().ToString("N").Substring(0, 20)
        //        };
        //        var createdOrder = await _orderRepo.CreateOrder(order);
        //        contract.Status = ContractStatusEnum.FirstPaymentPending.ToString();
        //        contract.UpdatedDate = DateTime.Now;
        //        await _contractRepo.UpdateContract(contract);

        //        var paymentInfo = new 
        //        {
        //            createdOrder.OrderId,
        //            createdOrder.OrderCode,
        //            contract.ContractId,
        //            bookingOffline.BookingOfflineId,
        //            Amount = firstPaymentAmount,
        //            PaymentType = "FirstPayment",
        //            createdOrder.Status,
        //            createdOrder.CreatedDate
        //        };
        //        res.IsSuccess = true;
        //        res.StatusCode = StatusCodes.Status200OK;
        //        res.Message = ResponseMessageConstrantsOrder.ORDER_STATUS_TO_PAID;
        //        res.Data = paymentInfo;
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi khi xử lý thanh toán lần đầu");
        //        res.IsSuccess = false;
        //        res.StatusCode = StatusCodes.Status500InternalServerError;
        //        res.Message = ex.Message;
        //        return res;
        //    }
        //}

        public async Task<ResultModel> SendOtpForContract(string contractId)
        {
            var res = new ResultModel();
            try
            {
                var contract = await _contractRepo.GetContractById(contractId);
                if (contract == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsContract.NOT_FOUND;
                    return res;
                }
                if (contract.Status != ContractStatusEnum.VerifyingOTP.ToString())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsContract.CHECK_STATUS;
                    return res;
                }
                var bookingOffline = contract.BookingOfflines.FirstOrDefault();
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }
                bookingOffline.Status = BookingOfflineEnums.VerifyingOTP.ToString();
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);

                //if (contract.OtpExpiredTime.HasValue && DateTime.Now > contract.OtpExpiredTime)
                //{
                //    contract.OtpCode = null;
                //    contract.OtpExpiredTime = null;
                //}
                // Generate OTP 6 số
                var random = new Random();
                string otp = random.Next(100000, 999999).ToString();
                contract.OtpCode = otp;
                contract.OtpExpiredTime = DateTime.Now.AddMinutes(1);
                contract.UpdatedDate = DateTime.Now;

                // Gửi OTP qua email
                var emailContent = $@"
                    <h2>Xác nhận hợp đồng</h2>
                    <p>Mã OTP của bạn là: <strong>{otp}</strong></p>
                    <p>Mã có hiệu lực trong vòng 60 giây.</p>
                    <p>Vui lòng không chia sẻ mã này với bất kỳ ai.</p>";

                await _emailService.SendEmail(
                    bookingOffline.Customer.Account.Email,
                    "Mã OTP xác nhận hợp đồng",
                    emailContent
                );
                await _contractRepo.UpdateContract(contract);
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status201Created;
                res.Message = ResponseMessageConstrantsContract.SEND_OTP_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi OTP");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> VerifyContractOtp(string contractId, VerifyOtpRequest request)
        {
            var res = new ResultModel();
            try
            {
                var contract = await _contractRepo.GetContractById(contractId);
                if (contract == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsContract.NOT_FOUND;
                    return res;
                }
                if (contract.Status != ContractStatusEnum.VerifyingOTP.ToString())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsContract.CHECK_STATUS;
                    return res;
                }

                if (DateTime.Now > contract.OtpExpiredTime)
                {
                    contract.OtpCode = null;
                    contract.OtpExpiredTime = null;
                    await _contractRepo.UpdateContract(contract);

                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsContract.OTP_EXPIRED;
                    return res;
                }

                if (contract.OtpCode != request.OtpCode)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsContract.VERIFY_OTP_FAILED;
                    return res;
                }
                var bookingOffline = contract.BookingOfflines.FirstOrDefault();
                if (bookingOffline == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE;
                    return res;
                }
                bookingOffline.Status = BookingOfflineEnums.VerifiedOTP.ToString();
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);

                contract.OtpCode = null;
                contract.OtpExpiredTime = null;
                contract.Status = ContractStatusEnum.FirstPaymentSuccess.ToString();
                contract.UpdatedDate = DateTime.Now;

                await _contractRepo.UpdateContract(contract);
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsContract.VERIFY_OTP_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi verify OTP");
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetContractById(string id)
        {
            var res = new ResultModel();
            try
            {
                var contract = await _contractRepo.GetContractById(id);
                if (contract != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<ContractResponse>(contract);
                    res.Message = ResponseMessageConstrantsContract.FOUND;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsContract.NOT_FOUND;
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

        public async Task<ResultModel> GetAllContracts()
        {
            var res = new ResultModel();
            try
            {
                var contracts = await _contractRepo.GetContracts();
                if (contracts != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<List<ContractResponse>>(contracts);
                    res.Message = ResponseMessageConstrantsContract.FOUND;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsContract.NOT_FOUND;
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

        public async Task<ResultModel> GetAllContractByStaff()
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

                var contracts = await _contractRepo.GetContractByStaffId(accountId);
                if (contracts != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<List<ContractResponse>>(contracts);
                    res.Message = ResponseMessageConstrantsContract.FOUND;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsContract.NOT_FOUND;
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
