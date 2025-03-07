using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.Extensions.Configuration;
using Services.ApiModels.Payment;
using Services.Interfaces;

namespace Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _apiKey;
        private readonly string _checksumKey;
        private readonly string _payosApiUrl;
        private readonly IPriceService _priceService;

        public PaymentService(
            IConfiguration configuration, 
            HttpClient httpClient,
            IPriceService priceService)
        {
            _httpClient = httpClient;
            _clientId = configuration["PayOs:ClientId"];
            _apiKey = configuration["PayOs:ApiKey"];
            _checksumKey = configuration["PayOs:ChecksumKey"];
            _payosApiUrl = configuration["PayOs:ApiUrl"];
            _priceService = priceService;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request)
        {
            try
            {
                // Validate and get price from service
                var servicePrice = await _priceService.GetServicePrice(request.PaymentType, request.ServiceId.ToString());
                if (!servicePrice.HasValue)
                {
                    throw new Exception("Service price not found");
                }

                // Validate that the requested amount matches the service price
                if (request.Amount != servicePrice.Value)
                {
                    throw new Exception($"Invalid payment amount. Expected: {servicePrice.Value}, Received: {request.Amount}");
                }

                var paymentData = new
                {
                    orderCode = request.OrderId,
                    amount = request.Amount,
                    description = request.Description,
                    cancelUrl = request.CancelUrl,
                    returnUrl = request.ReturnUrl,
                    signature = GenerateSignature(request),
                    buyerName = request.CustomerName,
                    buyerEmail = request.CustomerEmail,
                    buyerPhone = request.CustomerPhone,
                    orderInfo = $"{request.PaymentType}_{request.ServiceId}"
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(paymentData),
                    Encoding.UTF8,
                    "application/json");

                _httpClient.DefaultRequestHeaders.Add("x-client-id", _clientId);
                _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

                var response = await _httpClient.PostAsync($"{_payosApiUrl}/v2/payment-requests", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<PaymentResponse>(responseContent);
                }

                throw new Exception($"Payment creation failed: {responseContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating payment: {ex.Message}");
            }
        }

        public async Task<PaymentResponse> CheckPaymentStatusAsync(string orderId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("x-client-id", _clientId);
                _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

                var response = await _httpClient.GetAsync($"{_payosApiUrl}/v2/payment-requests/{orderId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<PaymentResponse>(responseContent);
                }

                throw new Exception($"Payment status check failed: {responseContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking payment status: {ex.Message}");
            }
        }

        private string GenerateSignature(PaymentRequest request)
        {
            // Implement signature generation according to PayOs documentation
            // This is a placeholder - you need to implement the actual signature generation logic
            var dataToSign = $"{request.OrderId}|{request.Amount}|{_checksumKey}";
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
} 