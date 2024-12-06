using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MultiShop.Catalog.Services.HourlyDealServices;
using System;
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

                        // Log the start of the hourly deal generation
                        _logger.LogInformation($"Starting hourly deal generation at {DateTime.UtcNow}");

                        // Generate new hourly deals
                        await hourlyDealService.GenerateHourlyDealsAsync();

                        // Clean up expired deals
                        await hourlyDealService.DeactivateExpiredHourlyDealsAsync();

                        _logger.LogInformation($"Completed hourly deal generation at {DateTime.UtcNow}");
                    }

                    // Wait for the next interval
                    await Task.Delay(_generateInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    // Log any unexpected errors
                    _logger.LogError(ex, "An error occurred during hourly deal background service execution");

                    // Wait a bit before retrying to prevent rapid error cycling
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