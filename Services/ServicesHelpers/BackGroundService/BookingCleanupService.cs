using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Services.BookingService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.ServicesHelpers.BackGroundService
{
    public class BookingCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BookingCleanupService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromHours(1); // Chạy mỗi giờ

        public BookingCleanupService(IServiceProvider services, ILogger<BookingCleanupService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BookingCleanupService đang chạy.");

            using var timer = new PeriodicTimer(_period);

            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    _logger.LogInformation("Đang thực hiện kiểm tra và hủy booking chưa thanh toán...");

                    using var scope = _services.CreateScope();
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                    var result = await bookingService.CancelUnpaidBookings();

                    _logger.LogInformation("Kết quả: {Message}", result.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi thực hiện hủy booking chưa thanh toán");
                }
            }
        }
    }
}