using DotNetChallenge.DTOs.SalesOrders;
using FluentValidation;

namespace DotNetChallenge.Validators.SalesOrders
{
    public class CreateSalesOrderValidator : AbstractValidator<CreateSalesOrderRequest>
    {
        public CreateSalesOrderValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("CustomerId is required.");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage(
                    "Sales order must contain at least one item.");

            RuleForEach(x => x.Items)
                .SetValidator(new SalesOrderItemValidator());

            RuleFor(x => x.Items)
                .Must(HaveUniqueProducts)
                .WithMessage("A product cannot appear more than once in a sales order.");
        }

        // Helper method to check for unique products in the sales order items
        private static bool HaveUniqueProducts(List<SalesOrderItemRequest> items)
        {
            return items
                .Select(x => x.ProductId)
                .Distinct()
                .Count() == items.Count;
        }
    }
}
