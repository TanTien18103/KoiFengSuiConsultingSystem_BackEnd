using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Services.OrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers.BackGroundService
{
    public class OrderExpirationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<OrderExpirationBackgroundService> _logger;

        public OrderExpirationBackgroundService(
            IServiceProvider services,
            ILogger<OrderExpirationBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    try
                    {
                        await orderService.CheckAndUpdateExpiredOrders();
                        _logger.LogInformation("Đã kiểm tra và cập nhật các đơn hàng hết hạn");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi kiểm tra đơn hàng hết hạn");
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
