using FluentValidation;
using DotNetChallenge.DTOs.Units;

namespace DotNetChallenge.Validators.Units
{
    public class CreateUnitValidator : AbstractValidator<CreateUnitRequest>
    {
        public CreateUnitValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Unit name is required.")
                .MaximumLength(100)
                .WithMessage("Unit name must not exceed 100 characters.");

            RuleFor(x => x.Symbol)
                .NotEmpty()
                .WithMessage("Unit symbol is required.")
                .MaximumLength(20)
                .WithMessage("Unit symbol must not exceed 20 characters.");
        }
    }
}
