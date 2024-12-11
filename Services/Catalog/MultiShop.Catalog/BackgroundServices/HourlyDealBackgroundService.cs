using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MultiShop.Catalog.Services.HourlyDealServices;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiShop.Catalog.BackgroundServices
{
    public class HourlyDealBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<HourlyDealBackgroundService> _logger;
        private readonly TimeSpan _generateInterval = TimeSpan.FromHours(3);
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5);

        public HourlyDealBackgroundService(
            IServiceProvider services,
            ILogger<HourlyDealBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var hourlyDealService = scope.ServiceProvider.GetRequiredService<IHourlyDealService>();

                        _logger.LogInformation($"Checking active hourly deals at {DateTime.UtcNow.AddHours(3)} (Local Time)");

                        // Aktif indirimlerin süresi doldu mu kontrol et
                        var activeDeals = await hourlyDealService.GetCurrentHourlyDealsAsync();
                        if (activeDeals.Any())
                        {
                            var earliestEndTime = activeDeals.Min(deal => deal.EndTime.AddHours(3)); // UTC+3
                            var delayDuration = earliestEndTime - DateTime.UtcNow.AddHours(3);

                            _logger.LogInformation($"Active deals found. Waiting until {earliestEndTime} (Local Time)");
                            if (delayDuration > TimeSpan.Zero)
                            {
                                await Task.Delay(delayDuration, stoppingToken);
                            }
                        }

                        _logger.LogInformation($"Starting hourly deal generation at {DateTime.UtcNow.AddHours(3)} (Local Time)");

                        // Yeni indirimleri oluştur
                        await hourlyDealService.GenerateHourlyDealsAsync();

                        // Süresi dolmuş indirimleri temizle
                        await hourlyDealService.DeactivateExpiredHourlyDealsAsync();

                        _logger.LogInformation($"Completed hourly deal generation at {DateTime.UtcNow.AddHours(3)} (Local Time)");
                    }

                    // Varsayılan interval
                    await Task.Delay(_generateInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during hourly deal background service execution");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hourly Deal Background Service is stopping.");
            await base.StopAsync(cancellationToken);
        }
    }
}