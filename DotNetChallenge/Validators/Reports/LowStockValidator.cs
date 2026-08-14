using DotNetChallenge.DTOs.Reports;
using FluentValidation;

namespace DotNetChallenge.Validators.Reports
{
    public class LowStockValidator : AbstractValidator<LowStockRequest>
    {
        public LowStockValidator()
        {
            RuleFor(x => x.Threshold)
                .GreaterThan(0)
                .WithMessage("Threshold must be greater than 0.");
        }
    }
}
