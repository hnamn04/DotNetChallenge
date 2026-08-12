using FluentValidation;
using DotNetChallenge.DTOs.Categories;

namespace DotNetChallenge.Validators.Categories 
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Category name is required.")
                .MaximumLength(100)
                .WithMessage("Category name must not exceed 100 characters.");
        }
    }
}
