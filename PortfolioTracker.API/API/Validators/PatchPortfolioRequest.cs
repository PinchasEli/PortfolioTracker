using API.DTOs.Portfolio;
using FluentValidation;

namespace API.Validators;

public class PatchPortfolioRequestValidator : AbstractValidator<PatchPortfolioRequest>
{
    public PatchPortfolioRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters")
            .When(x => x.Name != null);

        RuleFor(x => x.Active)
            .Must(x => x == true || x == false).WithMessage("Active status must be true or false")
            .When(x => x.Active != null);

        RuleFor(x => x)
            .Must(x => x.Name != null || x.Active != null)
            .WithMessage("At least one field must be provided for update");
    }
}