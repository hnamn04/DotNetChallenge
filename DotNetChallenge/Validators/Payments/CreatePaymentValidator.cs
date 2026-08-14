using DotNetChallenge.DTOs.Payments;
using FluentValidation;

namespace DotNetChallenge.Validators.Payments
{
    public class CreatePaymentValidator : AbstractValidator<CreatePaymentRequest>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0.");
            RuleFor(x => x.Method)
                .NotEmpty()
                .WithMessage("Method is required.")
                .MaximumLength(50)
                .WithMessage("Method must not exceed 50 characters.");
        }
    }
}
