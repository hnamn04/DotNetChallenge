namespace DotNetChallenge.Configuration
{
    public class DailySummaryJobSettings
    {
        public int RunAtHour { get; set; }
        public int RunAtMinute { get; set; }
        public string TimeZoneId { get; set; } = "UTC";
    }
}
