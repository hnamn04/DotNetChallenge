using DotNetChallenge.DTOs.Jobs;

namespace DotNetChallenge.Services.Jobs
{
    public interface IDailySummaryService
    {
        Task<DailySummaryResponse> GenerateDailySummaryAsync(DateOnly date);
    }
}
