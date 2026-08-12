using FluentValidation;
using DotNetChallenge.DTOs.Products;

namespace DotNetChallenge.Validators.Products
{
    public class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Product code is required.")
                .MaximumLength(50)
                .WithMessage("Product code must not exceed 50 characters.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .MaximumLength(200)
                .WithMessage("Product name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Product description must not exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.CostPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Cost price must be greater than or equal to 0.");

            RuleFor(x => x.SellingPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Selling price must be greater than or equal to 0.");

            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("Category is required.");

            RuleFor(x => x.UnitId)
                .NotEmpty()
                .WithMessage("Unit is required.");
        }
    }
}
