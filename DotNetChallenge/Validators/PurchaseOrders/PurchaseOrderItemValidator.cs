using DotNetChallenge.DTOs.PurchaseOrders;
using FluentValidation;
namespace DotNetChallenge.Validators.PurchaseOrders
{
    public class PurchaseOrderItemValidator : AbstractValidator<PurchaseOrderItemRequest>
    {
        public PurchaseOrderItemValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage(
                    "Quantity must be greater than 0.");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage(
                    "Unit price must be greater than or equal to 0.");
        }
    }
}
