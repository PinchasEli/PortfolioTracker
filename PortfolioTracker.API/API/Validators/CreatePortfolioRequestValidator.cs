using FluentValidation;
using API.DTOs.Portfolio;

using Domain.Enums;

namespace API.Validators;

public class CreatePortfolioRequestValidator : AbstractValidator<CreatePortfolioRequest>
{
    public CreatePortfolioRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters");

        RuleFor(x => x.Exchange)
            .NotEmpty().WithMessage("Exchange is required")
            .IsInEnum().WithMessage("Invalid exchange");
    }
}