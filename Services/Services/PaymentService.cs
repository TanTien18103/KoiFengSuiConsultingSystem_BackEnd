using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using Services.ApiModels.Payment;
using Services.Interfaces;
using System.Security.Cryptography;
using BusinessObjects.Models;
using Repositories.Repository;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using WebhookData = Net.payOS.Types.WebhookData;

public class PaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _apiKey;
    private readonly string _checksumKey;
    private readonly string _payosApiUrl;
    private readonly IPriceService _priceService;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICustomerRepo _customerRepo;
    private readonly ICourseRepo _courseRepo;
    private readonly IBookingOnlineRepo _bookingOnlineRepo;
    private readonly IBookingOfflineRepo _bookingOfflineRepo;
    private readonly IWorkShopRepo _workShopRepo;
    private readonly IOrderRepo _orderRepo;
    private readonly IAccountRepo _accountRepo;
    private const string PAYMENT_DESCRIPTION = "BitKoi dich vu";

    public PaymentService(
        IConfiguration configuration,
        HttpClient httpClient,
        IPriceService priceService,
        IHttpContextAccessor httpContextAccessor,
        ICustomerRepo customerRepo,
        ICourseRepo courseRepo,
        IBookingOnlineRepo bookingOnlineRepo,
        IBookingOfflineRepo bookingOfflineRepo,
        IWorkShopRepo workShopRepo,
        IOrderRepo orderRepo,
        IAccountRepo accountRepo)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _clientId = configuration["PayOs:ClientId"];
        _apiKey = configuration["PayOs:ApiKey"];
        _checksumKey = configuration["PayOs:ChecksumKey"];
        _payosApiUrl = configuration["PayOs:ApiUrl"];
        _priceService = priceService;
        _httpContextAccessor = httpContextAccessor;
        _customerRepo = customerRepo;
        _courseRepo = courseRepo;
        _bookingOnlineRepo = bookingOnlineRepo;
        _bookingOfflineRepo = bookingOfflineRepo;
        _workShopRepo = workShopRepo;
        _orderRepo = orderRepo;
        _accountRepo = accountRepo;
    }
    public static string GenerateShortGuid()
    {
        Guid guid = Guid.NewGuid();
        string base64 = Convert.ToBase64String(guid.ToByteArray());
        return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
    }
    public async Task<CreatePaymentResult> CreatePaymentLinkAsync(PayOSRequest request)
    {
        var order = await _orderRepo.GetOrderById(request.OrderId);

        var payOS = new PayOS(_clientId, _apiKey, _checksumKey);
        // Create an item with the order ID and customer name
        ItemData item = new ItemData($"{PAYMENT_DESCRIPTION} {order.OrderId} cho khach hang {order.Customer.Account.FullName}",
            1, (int) /*order.Total/100000*/ 2000);
        List<ItemData> items = new List<ItemData>();
        items.Add(item);

        // ConvertDateTimeToUnixTimestamp
        var expiredAt = ConvertDateTimeToUnixTimestamp(DateTime.Now.AddMinutes(15));
        int orderCode = int.Parse(DateTime.Now.ToString("ffffff"));

        // Create a PaymentData object
        PaymentData paymentData = new PaymentData(orderCode, (int) /*order.Total/100000*/ 2000, $"{PAYMENT_DESCRIPTION} {order.OrderId}",
            items, request.CancelUrl, request.ReturnUrl, expiredAt: expiredAt);

        // Create a signature for the payment data
        var signature = CreateSignature(payOS, paymentData);
        paymentData = paymentData with { signature = signature };

        CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);

        // update order
        order.Note = orderCode.ToString();
        order.Status = PaymentStatusEnums.Pending.ToString();
        await _orderRepo.UpdateOrder(order);
        return createPayment;
    }

    private string CreateSignature(PayOS payOs, PaymentData paymentData)
    {
        // Prepare data by extracting properties from the PaymentData object, sorted by property name
        var properties = paymentData.GetType().GetProperties()
            .OrderBy(p => p.Name)
            // Ensure value is not null
            .Where(p => p.GetValue(paymentData) != null);

        StringBuilder sb = new StringBuilder();

        // Build the query string (key1=value1&key2=value2...)
        foreach (var property in properties)
        {
            var value = property.GetValue(paymentData)?.ToString();
            // Skip if the value is an empty string
            if (!string.IsNullOrEmpty(value))
            {
                sb.Append($"{property.Name}={value}&");
            }
        }

        // Remove the last '&' if it exists
        if (sb.Length > 0)
        {
            // Efficient way to remove the trailing '&'
            sb.Length--;
        }

        // Encrypt the resulting string using HMAC-SHA256
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            // Convert hash to lowercase hexadecimal
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    private int ConvertDateTimeToUnixTimestamp(DateTimeOffset dateTime)
    {
        return (int)dateTime.ToUnixTimeSeconds();
    }

    public async Task GetWebhookTypeAsync(WebhookType request)
    {
        var orderCode = request.data.orderCode;
        var payOs = new PayOS(_clientId, _apiKey, _checksumKey);
        WebhookData data = payOs.verifyPaymentWebhookData(request);
        if (data == null)
        {
            throw new Exception("Webhook data not found");
        }

        var orderId = request.data.description.Split(" ").Last();

        // valid data & change status of order
        var order = await _orderRepo.GetOrderById(orderId);
        if (order == null)
        {
            throw new Exception($"Order {orderId} not found");
        }

        // if status == "00" change status of order to success
        if (request.code == "00")
        {
            await UpdateOrderPayment(order, data.orderCode);
        }
        else
        {
            throw new Exception($"Request failed for order {orderId}");
        }
    }

    public async Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderCode)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-client-id", _clientId);
        client.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        var response = await client.GetAsync($"https://api-merchant.payos.vn/v2/payment-requests/{orderCode}");
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            var payOsResponse = JsonConvert.DeserializeObject<PayOSResponse>(content);
            return payOsResponse.Data;
        }
        throw new Exception("Không tìm thấy dịch vụ cần thanh toán");
    }

    public async Task ConfirmPayment(string orderId, long orderCode)
    {
        var order = await _orderRepo.GetOrderById(orderId);
        if (order.Status == PaymentStatusEnums.Paid.ToString())
        {
            throw new Exception("Đã thanh toán");
        }

        var payment = await GetPaymentLinkInformationAsync(orderCode);
        if (payment == null)
        {
            throw new Exception("Không tìm thấy dịch vụ cần thanh toán");
        }

        if (payment.status != "PAID")
        {
            throw new Exception("Chưa thanh toán");
        }
        await UpdateOrderPayment(order, orderCode);
    }

    private async Task UpdateOrderPayment(Order order, long orderCode)
    {
        order.Status = PaymentStatusEnums.Paid.ToString();
        order.PaymentDate = DateTime.Now;
        order.PaymentId = orderCode.ToString();
        await _orderRepo.UpdateOrder(order);
    }

    public async Task<string> ConfirmWebhook(string webhookUrl)
    {
        var payOs = new PayOS(_clientId, _apiKey, _checksumKey);
        return await payOs.confirmWebhook(webhookUrl);
    }
}