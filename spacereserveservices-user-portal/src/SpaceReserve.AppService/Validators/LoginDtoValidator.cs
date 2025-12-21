using FluentValidation;
using SpaceReserve.AppService.DTOs;

namespace SpaceReserve.AppService.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    [Obsolete]
    public LoginDtoValidator()
    {
        CascadeMode = CascadeMode.Stop;
        
        RuleFor(s => s.SubjectId)
            .NotEmpty()
            .WithMessage("Subject ID is required.")
            .MaximumLength(100)
            .WithMessage("Subject ID must not exceed 100 characters.")
            .Must(x => !x.Contains(" "))
            .WithMessage("Subject ID must not include white space.");
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()   
            .WithMessage("Invalid email format.")
            .MaximumLength(100)
            .WithMessage("Email must not exceed 100 characters.")
            .Must(x => x.EndsWith("@1rivet.com"))
            .WithMessage("Please enter a valid domain email address.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(50)
            .WithMessage("First name must not exceed 50 characters.")
            .Must(x => !x.Contains(" "))
            .WithMessage("First Name must not include white space."); 

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(50)
            .WithMessage("Last name must not exceed 50 characters.")
            .Must(x => !x.Contains(" "))
            .WithMessage("Last Name must not include white space");
    }
}
