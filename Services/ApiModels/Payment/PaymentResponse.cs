using System;

namespace BusinessObjects
{
    public class PaymentResponse
    {
        public string Id { get; set; }  // ID của giao dịch từ PayOs
        public string OrderId { get; set; }  // Mã đơn hàng của bạn
        public decimal Amount { get; set; }  // Số tiền thanh toán
        public string Description { get; set; }  // Mô tả giao dịch
        public string Status { get; set; }  // Trạng thái giao dịch: PENDING, COMPLETED, CONFIRMED, CANCELLED
        public DateTime CreatedAt { get; set; }  // Thời gian tạo giao dịch
        public DateTime? PaidAt { get; set; }  // Thời gian thanh toán thành công
        public string PaymentUrl { get; set; }  // URL thanh toán
        public string QrCode { get; set; }  // Mã QR để quét thanh toán
        public string DeepLink { get; set; }  // Link để mở app thanh toán trên mobile
        public PaymentMethodResponse PaymentMethod { get; set; }  // Thông tin phương thức thanh toán
        public string Message { get; set; }  // Thông báo từ PayOs
    }

    public class PaymentMethodResponse
    {
        public string Type { get; set; }  // Loại phương thức thanh toán (BANK_TRANSFER, E_WALLET, etc.)
        public string Code { get; set; }  // Mã phương thức thanh toán
        public string Name { get; set; }  // Tên phương thức thanh toán
        public BankResponse Bank { get; set; }  // Thông tin ngân hàng (nếu có)
    }

    public class BankResponse
    {
        public string Id { get; set; }  // ID ngân hàng
        public string Name { get; set; }  // Tên ngân hàng
        public string Code { get; set; }  // Mã ngân hàng
        public string AccountNo { get; set; }  // Số tài khoản
        public string AccountName { get; set; }  // Tên tài khoản
    }
} 