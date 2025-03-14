using Microsoft.Extensions.Logging;
using Net.payOS.Types;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Services.ApiModels.Payment;

namespace Services.Services.PaymentService
{
    public class PayOSService : IPayOSService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PayOSService> _logger; // Thêm logger vào lớp

        // Cập nhật constructor để inject ILogger
        public PayOSService(IConfiguration config, ILogger<PayOSService> logger)
        {
            _config = config;
            _logger = logger;  // Gán giá trị logger vào biến _logger
        }

        public async Task<CreatePaymentResult> CreatePaymentUrl(PayOSRequest payOSReqModel)
        {
            // Ghi log khi bắt đầu phương thức
            _logger.LogInformation("Dang tao URL thanh toan cho OrderId: {OrderId}, So tien: {Amount}", payOSReqModel.OrderId, payOSReqModel.Amount);

            try
            {
                // Khởi tạo client PayOS với các giá trị cấu hình
                PayOS payOS = new PayOS(
                    _config["PayOS:ClientID"],
                    _config["PayOS:ApiKey"],
                    _config["PayOS:ChecksumKey"]
                );

                _logger.LogInformation("Da khoi tao client PayOS voi ClientID: {ClientID}", _config["PayOS:ClientID"]);

                // Tạo item thanh toán
                ItemData item = new ItemData(payOSReqModel.SubName, 1, (int)payOSReqModel.Amount);
                List<ItemData> items = new List<ItemData> { item };

                _logger.LogInformation("Da tao item thanh toan voi SubName: {SubName}, So tien: {Amount}", payOSReqModel.SubName, payOSReqModel.Amount);

                // Chuẩn bị dữ liệu thanh toán
                PaymentData paymentData = new PaymentData(
                    payOSReqModel.OrderId,
                    (int)payOSReqModel.Amount,
                    payOSReqModel.SubName,
                    items,
                    payOSReqModel.CancleUrl,
                    payOSReqModel.RedirectUrl
                );

                // Log payment data with correct property names
                _logger.LogInformation("Du lieu thanh toan da duoc chuan bi cho OrderCode: {OrderCode}, Amount: {Amount}, Description: {Description}, CancelUrl: {CancelUrl}, ReturnUrl: {ReturnUrl}",
                    paymentData.orderCode, // Use the correct property name
                    paymentData.amount,
                    paymentData.description,
                    paymentData.cancelUrl, // Corrected from 'CancleUrl'
                    paymentData.returnUrl); // Corrected from 'RedirectUrl'

                // Log item details
                // Assuming 'ItemData' contains 'name', 'quantity', and 'price' as properties
                foreach (var paymentItem in paymentData.items)
                {
                    _logger.LogInformation("ItemData: Name: {Name}, Quantity: {Quantity}, Price: {Price}",
                        paymentItem.name,      // Use the correct property name for description or name
                        paymentItem.quantity,  // Assuming 'quantity' exists
                        paymentItem.price);    // Assuming 'price' exists
                }



                _logger.LogInformation("Du lieu thanh toan da duoc chuan bi cho OrderId: {OrderId}", payOSReqModel.OrderId);

                // Gọi PayOS để tạo link thanh toán
                CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);

                if (createPayment != null && createPayment.status == "PENDING")
                {
                    // Ghi log thành công
                    _logger.LogInformation("Da tao URL thanh toan cho OrderId: {OrderId}", payOSReqModel.OrderId);
                }
                else
                {
                    // Ghi log thất bại nếu việc tạo link thanh toán không thành công
                    _logger.LogWarning("Tao URL thanh toan that bai cho OrderId: {OrderId}", payOSReqModel.OrderId);
                }

                // Trả về kết quả
                return createPayment;
            }
            catch (Exception ex)
            {
                // Ghi log khi có lỗi xảy ra
                _logger.LogError(ex, "Đã xảy ra lỗi khi tạo URL thanh toán cho OrderId: {OrderId}", payOSReqModel.OrderId);
                throw; // Ném lại ngoại lệ để xử lý ở nơi khác nếu cần
            }
        }
    }
}
