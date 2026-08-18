using DotNetChallenge.DTOs.Reports;
using FluentValidation;

namespace DotNetChallenge.Validators.Reports
{
    public class SalesExportQueryRequestValidator : AbstractValidator<SalesExportQueryRequest>
    {
        public SalesExportQueryRequestValidator()
        {
            RuleFor(x => x.ToDate)
                .GreaterThanOrEqualTo(x => x.FromDate)
                .WithMessage("ToDate cannot be earlier than FromDate.");

            RuleFor(x => x)
                .Must(BeWithin31Days)
                .WithMessage("Export date range cannot exceed 31 days to prevent memory overload.");
        }

        private bool BeWithin31Days(SalesExportQueryRequest request)
        {
            var totalDays = (request.ToDate - request.FromDate).TotalDays;
            return totalDays <= 31;
        }
    }
}