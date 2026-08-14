using DotNetChallenge.DTOs.SalesOrders;
using FluentValidation;

namespace DotNetChallenge.Validators.SalesOrders
{
    public class SalesOrderItemValidator : AbstractValidator<SalesOrderItemRequest>
    {
        public SalesOrderItemValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");
        }
    }
}
