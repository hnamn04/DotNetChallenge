using DotNetChallenge.DTOs.Customers;
using FluentValidation;

namespace DotNetChallenge.Validators.Customers
{
    public class CustomerQueryValidator : AbstractValidator<CustomerQueryRequest>
    {
        public CustomerQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1.");

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100)
                .WithMessage("Limit must be between 1 and 100.");
        }
    }
}
