using DotNetChallenge.Services.Jobs;

namespace DotNetChallenge.BackgroundJobs
{
    public class DailySummaryBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DailySummaryBackgroundService> _logger;

        public DailySummaryBackgroundService(IServiceScopeFactory scopeFactory, ILogger<DailySummaryBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;

                    // Run once every day at 00:00
                    var nextRun = now.Date.AddDays(1);

                    var delay = nextRun - now;

                    await Task.Delay(delay, stoppingToken);

                    using var scope = _scopeFactory.CreateScope();

                    var service = scope.ServiceProvider
                        .GetRequiredService<IDailySummaryService>();

                    var date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

                    await service.GenerateDailySummaryAsync(date);

                    _logger.LogInformation("Daily summary background job completed.");
                }
                catch (OperationCanceledException)
                {
                    // Application is shutting down
                }
                catch (Exception ex) 
                { 
                    _logger.LogError(ex, "Daily summary background job failed."); 
                } 
            }
        }
    }
}
