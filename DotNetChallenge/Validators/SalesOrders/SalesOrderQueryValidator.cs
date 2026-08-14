using DotNetChallenge.DTOs.SalesOrders;
using FluentValidation;

namespace DotNetChallenge.Validators.SalesOrders
{
    public class SalesOrderQueryValidator : AbstractValidator<SalesOrderQueryRequest>
    {
        public SalesOrderQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1.");

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100)
                .WithMessage("Limit must be between 1 and 100.");

            RuleFor(x => x)
                .Must(x =>
                    !x.FromDate.HasValue ||
                    !x.ToDate.HasValue ||
                    x.FromDate <= x.ToDate)
                .WithMessage("FromDate must be less than or equal to ToDate.");
        }
    }
}
