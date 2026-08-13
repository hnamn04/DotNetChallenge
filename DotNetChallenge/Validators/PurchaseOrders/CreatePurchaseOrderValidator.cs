using DotNetChallenge.DTOs.PurchaseOrders;
using FluentValidation;

namespace DotNetChallenge.Validators.PurchaseOrders
{
    public class CreatePurchaseOrderValidator : AbstractValidator<CreatePurchaseOrderRequest>
    {
        public CreatePurchaseOrderValidator()
        {
            RuleFor(x => x.SupplierId)
                .NotEmpty()
                .WithMessage("SupplierId is required.");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage(
                    "Purchase order must contain at least one item.");

            RuleForEach(x => x.Items)
                .SetValidator(new PurchaseOrderItemValidator());
        }
    }
}
