using DotNetChallenge.DTOs.Suppliers;
using FluentValidation;

namespace DotNetChallenge.Validators.Suppliers
{
    public class CreateSupplierRequestValidator
    : AbstractValidator<CreateSupplierRequest>
    {
        public CreateSupplierRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Supplier name is required.")
                .MaximumLength(200)
                .WithMessage("Supplier name must not exceed 200 characters.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Email format is invalid.")
                .MaximumLength(255)
                .WithMessage("Email must not exceed 255 characters.");

            RuleFor(x => x.Phone)
                .MaximumLength(50)
                .WithMessage("Phone must not exceed 50 characters.");

            RuleFor(x => x.Address)
                .MaximumLength(500)
                .WithMessage("Address must not exceed 500 characters.");
        }
    }
}
