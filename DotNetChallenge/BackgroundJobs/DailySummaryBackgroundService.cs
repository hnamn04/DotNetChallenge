using DotNetChallenge.Configuration;
using DotNetChallenge.Services.Jobs;
using Microsoft.Extensions.Options;

namespace DotNetChallenge.BackgroundJobs
{
    public class DailySummaryBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DailySummaryBackgroundService> _logger;
        private readonly DailySummaryJobSettings _jobSettings;

        public DailySummaryBackgroundService(
            IServiceScopeFactory scopeFactory, 
            ILogger<DailySummaryBackgroundService> logger,
            IOptions<DailySummaryJobSettings> options)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _jobSettings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Get configuration for the daily summary job
                    var timeZoneId = _jobSettings.TimeZoneId ?? "UTC";
                    var runHour = _jobSettings.RunAtHour;
                    var runMinute = _jobSettings.RunAtMinute;

                    // Get the current time in the specified time zone
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                    var nowUtc = DateTime.UtcNow;
                    var nowInZone = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZone);

                    // Calculate the next run time in the specified time zone
                    var nextRunInZone = nowInZone.Date.AddHours(runHour).AddMinutes(runMinute);

                    // Check if the next run time is in the past; if so, schedule for the next day
                    if (nowInZone >= nextRunInZone)
                    {
                        nextRunInZone = nextRunInZone.AddDays(1);
                    }

                    // Convert the next run time back to UTC for the delay calculation
                    var nextRunUtc = TimeZoneInfo.ConvertTimeToUtc(nextRunInZone, timeZone);
                    var delay = nextRunUtc - nowUtc;

                    _logger.LogInformation(
                        "Next daily summary job will run at {NextRunInZone} ({TimeZoneId}). Delaying for {Delay}", 
                        nextRunInZone, timeZoneId, delay);

                    // Wait until the next scheduled run time
                    await Task.Delay(delay, stoppingToken);

                    // Create a new scope to resolve the service
                    using var scope = _scopeFactory.CreateScope();

                    var service = scope.ServiceProvider.GetRequiredService<IDailySummaryService>();

                    // Determine the date for the report based on the actual run time:
                    var currentUtc = DateTime.UtcNow;
                    var currentInZone = TimeZoneInfo.ConvertTimeFromUtc(currentUtc, timeZone);

                    // If the job runs before noon, generate the summary for the previous day; otherwise, generate for today
                    var dateInZone = currentInZone.Hour < 12 
                        ? currentInZone.AddDays(-1) 
                        : currentInZone;

                    var date = DateOnly.FromDateTime(dateInZone);

                    await service.GenerateDailySummaryAsync(date);

                    _logger.LogInformation("Daily summary background job completed for date {Date}.", date);
                }
                catch (OperationCanceledException)
                {
                    // Application is shutting down
                }
                catch (Exception ex) 
                { 
                    _logger.LogError(ex, "Daily summary background job failed."); 
                    
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                } 
            }
        }
    }
}