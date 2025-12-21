using FluentValidation;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Utility.Enum;
using SpaceReserve.Utility.Resources;

namespace SpaceReserve.AppService.Validators;

public class BookingHistoryValidator : AbstractValidator<BookingHistoryQueryDto>
{
    [Obsolete]
    public BookingHistoryValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Sort)
            .Must(x => x == (int)BookingStatus.All || x == (int)BookingStatus.Pending || x == (int)BookingStatus.Accepted || x == (int)BookingStatus.Rejected || x == (int)BookingStatus.Cancelled)
            .WithMessage("Sort order must be 0, 1, 2, 3, or 4.");

    }
} 
