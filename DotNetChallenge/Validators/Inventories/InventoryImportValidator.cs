using DotNetChallenge.DTOs.Inventories;
using FluentValidation;

namespace DotNetChallenge.Validators.Inventories
{
    public class InventoryImportValidator : AbstractValidator<InventoryImportRequest>
    {
        public InventoryImportValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.Note)
                .NotEmpty()
                .WithMessage("Note is required.")
                .MaximumLength(1000)
                .WithMessage("Note must not exceed 1000 characters.");
        }
    }
}
