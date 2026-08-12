using DotNetChallenge.DTOs.Customers;
using FluentValidation;

namespace DotNetChallenge.Validators.Customers
{
    public class UpdateCustomerRequestValidator
    : AbstractValidator<UpdateCustomerRequest>
    {
        public UpdateCustomerRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Customer name is required.")
                .MaximumLength(200)
                .WithMessage("Customer name must not exceed 200 characters.");

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