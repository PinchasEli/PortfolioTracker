using API.DTOs.Customers;
using FluentValidation;

namespace API.Validators;

public class PatchCustomerRequestValidator : AbstractValidator<PatchCustomerRequest>
{
    public PatchCustomerRequestValidator()
    {
        // Only validate fields that are provided (not null)
        
        RuleFor(x => x.FullName)
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters")
            .When(x => x.FullName != null);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters")
            .When(x => x.Email != null);

        // Ensure at least one field is provided
        RuleFor(x => x)
            .Must(x => x.FullName != null || x.Email != null || x.Active != null)
            .WithMessage("At least one field must be provided for update");
    }
}

