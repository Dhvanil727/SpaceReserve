using FluentValidation;
using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Admin.Utility.Resources;

namespace SpaceReserve.Admin.AppService.Validators;

public class RequestHistoryActionValidator : AbstractValidator<RequestHistoryActionDtoValidator>
{
    [Obsolete]
    public RequestHistoryActionValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty()
            .WithMessage("Request ID is required.")
            .GreaterThan(0)
            .WithMessage("Request ID must be greater than 0.");

        RuleFor(x => x.Action)
            .NotEmpty()
            .WithMessage("Action is required.")
            .Must(x => x == (byte)CommonResources.BookingStatus.Accepted || x == (byte)CommonResources.BookingStatus.Rejected)
            .WithMessage("Action must be either 2 (Approve) or 3 (Reject).");

       
    }
}

