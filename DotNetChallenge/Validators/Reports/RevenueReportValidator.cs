using DotNetChallenge.DTOs.Reports;
using FluentValidation;

namespace DotNetChallenge.Validators.Reports
{
    public class RevenueReportValidator : AbstractValidator<RevenueReportRequest>
    {
        public RevenueReportValidator()
        {
            RuleFor(x => x)
                .Must(x => x.FromDate <= x.ToDate)
                .WithMessage("FromDate must be less than or equal to ToDate.");
        }
    }
}
