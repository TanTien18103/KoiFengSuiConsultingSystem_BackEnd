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
using System.Net.Http.Json;

namespace Services.Services.PaymentService
{
    public class PayOSService : IPayOSService
    {
        private readonly ILogger<PayOSService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PayOSService(
            ILogger<PayOSService> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient("PayOS");
            
            // Cấu hình HttpClient
            _httpClient.BaseAddress = new Uri(_configuration["PayOs:ApiUrl"]);
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _configuration["PayOs:ClientId"]);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _configuration["PayOs:ApiKey"]);
        }

        public async Task<PayOSResponse> CreatePaymentUrl(PayOSRequest request)
        {
            try
            {
                var payload = new
                {
                    orderCode = request.OrderId,
                    amount = (int)request.Amount,
                    description = request.Description,
                    returnUrl = request.ReturnUrl,
                    cancelUrl = request.CancelUrl,
                    customerName = request.CustomerName
                };

                var response = await _httpClient.PostAsJsonAsync("/v2/payment-requests", payload);
                response.EnsureSuccessStatusCode();
                
                var result = await response.Content.ReadFromJsonAsync<PayOSResponse>();
                
                if (result?.Status == "PENDING")
                {
                    _logger.LogInformation("Đã tạo URL thanh toán cho OrderId: {OrderId}", request.OrderId);
                }
                else
                {
                    _logger.LogWarning("Tạo URL thanh toán thất bại cho OrderId: {OrderId}", request.OrderId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi tạo URL thanh toán cho OrderId: {OrderId}", request.OrderId);
                throw;
            }
        }
    }
}
