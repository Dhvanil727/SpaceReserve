using FluentValidation;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Utility.Enum;

namespace SpaceReserve.AppService.Validators;

public class RequestHistoryStatusValidator : AbstractValidator<RequestHistoryStatusValidatorDto>
{
    [Obsolete]
    public RequestHistoryStatusValidator()
    {
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Action)
            .NotEmpty()
            .WithMessage("Action is required.")
            .Must(x => x > 0)
            .WithMessage("RequestId must be a positive integer.")
            .Must(x => x == (byte)BookingStatus.Accepted || x == (byte)BookingStatus.Rejected)
            .WithMessage("Action must be either 2 or 3.");

        RuleFor(x => x.RequestId)
            .NotEmpty()
            .WithMessage("RequestId is required.")
            .Must(x => x > 0)
            .WithMessage("RequestId must be a positive integer.");
    }
}

