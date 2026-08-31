using BPS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Infrastructure.BackgroundServices
{
    public class SeatLockCleanupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SeatLockCleanupWorker> _logger;

        public SeatLockCleanupWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<SeatLockCleanupWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Seat Lock Cleanup Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope =
                        _scopeFactory.CreateScope();

                    var bookingRepository =
                        scope.ServiceProvider
                            .GetRequiredService<IBookingRepository>();

                    var releasedCount =
                        await bookingRepository
                            .ReleaseExpiredSeatLocksAsync();

                    if (releasedCount > 0)
                    {
                        _logger.LogInformation(
                            "Released {Count} expired seat locks.",
                            releasedCount);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error while releasing expired seat locks.");
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(60),
                    stoppingToken);
            }

            _logger.LogInformation(
                "Seat Lock Cleanup Worker stopped.");
        }
    }
}
