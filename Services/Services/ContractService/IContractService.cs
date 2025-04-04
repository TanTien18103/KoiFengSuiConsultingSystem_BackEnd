using Services.ApiModels.Contract;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.ContractService
{
    public interface IContractService
    {
        Task<ResultModel> CreateContract(ContractRequest request);
        Task<ResultModel> SendOtpForContract(string contractId);
        Task<ResultModel> VerifyContractOtp(string contractId, VerifyOtpRequest request);
        Task<ResultModel> CancelContractByManager(string contractId);
        Task<ResultModel> ConfirmContractByManager(string contractId);
        Task<ResultModel> CancelContractByCustomer(string contractId);
        Task<ResultModel> ConfirmContractByCustomer(string contractId);
        Task<ResultModel> GetContractByBookingOfflineIdAndUpdateStatus(string bookingOfflineId);
        //Task<ResultModel> ProcessFirstPaymentAfterVerification(string contractId);
        Task<ResultModel> GetContractByBookingOfflineId(string bookingOfflineId);
        Task<ResultModel> GetContractById(string id);
        Task<ResultModel> GetAllContracts();
        Task<ResultModel> GetAllContractByStaff();
    }
}
