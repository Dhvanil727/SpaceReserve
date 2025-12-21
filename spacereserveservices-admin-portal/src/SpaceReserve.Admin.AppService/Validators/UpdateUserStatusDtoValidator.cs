using FluentValidation;
using SpaceReserve.Admin.AppService.DTOs;

namespace SpaceReserve.Admin.AppService.Validators;

public class UpdateUserStatusDtoValidator : AbstractValidator<UpdateUserStatusDto>
{
    public UpdateUserStatusDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.")
            .Must(id => int.TryParse(id.ToString(), out _))
            .WithMessage("UserId must be an Integer")
            .GreaterThan(0)
            .WithMessage("UserId must be a positive integer.");

        RuleFor(s => s.SubjectId)
            .NotEmpty()
            .WithMessage("Subject ID is required.")
            .MaximumLength(100)
            .WithMessage("Subject ID must not exceed 100 characters.")
            .Must(x => !x.Contains(" "))
            .WithMessage("Subject ID must not include white space.");

        RuleFor(x => x.IsActive)
            .NotNull()
            .WithMessage("IsActive is required.")
            .Must(value => value.GetTypeCode() == TypeCode.Boolean)
            .WithMessage("IsActive must be a boolean value (true or false).");
    }
}
