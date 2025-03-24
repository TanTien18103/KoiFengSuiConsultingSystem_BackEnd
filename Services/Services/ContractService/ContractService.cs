using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repositories.Repositories.BookingOfflineRepository;
using Repositories.Repositories.ContractRepository;
using Services.ApiModels;
using Services.ApiModels.Contract;
using Services.Services.EmailService;
using Services.ServicesHelpers.UploadService;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.Services.ContractService
{

    public class ContractService : IContractService
    {
        private readonly IContractRepo _contractRepo;
        private readonly IBookingOfflineRepo _bookingOfflineRepo;
        private readonly IEmailService _emailService;
        private readonly IUploadService _uploadService;
        private readonly IMapper _mapper;
        private readonly ILogger<ContractService> _logger;
        public ContractService(
        IContractRepo contractRepo,
        IBookingOfflineRepo bookingOfflineRepo,
        IEmailService emailService,
        IUploadService uploadService,
        IMapper mapper,
        ILogger<ContractService> logger)
        {
            _contractRepo = contractRepo;
            _bookingOfflineRepo = bookingOfflineRepo;
            _emailService = emailService;
            _uploadService = uploadService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultModel> CancelContract(string contractId)
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
                contract.Status = ContractStatusEnum.Cancelled.ToString();
                contract.UpdatedDate = DateTime.Now;
                bookingOffline.ContractId = null;
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

        public async Task<ResultModel> ConfirmContract(string contractId)
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
                if(contract == null)
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

                var contract = new BusinessObjects.Models.Contract
                {
                    ContractId = Guid.NewGuid().ToString("N").Substring(0, 20),
                    Status = ContractStatusEnum.Pending.ToString(),
                    ContractName = $"Contract_{request.BookingOfflineId}_{DateTime.Now:yyyyMMdd}",
                    DocNo = $"DOC_{DateTime.Now:yyyyMMddHHmmss}",
                    CreatedDate = DateTime.Now,
                    ContractUrl = tempPdfUrl
                };

                var createdContract = await _contractRepo.CreateContract(contract);

                // Update booking với contractId
                bookingOffline.ContractId = contract.ContractId;
                await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);

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
                    UpdatedDate = DateTime.Now
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

                contract.OtpCode = null;
                contract.OtpExpiredTime = null;
                contract.Status = ContractStatusEnum.Success.ToString();
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
    }
}
