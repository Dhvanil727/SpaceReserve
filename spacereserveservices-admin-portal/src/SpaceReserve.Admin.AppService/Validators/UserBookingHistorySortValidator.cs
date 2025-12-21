using FluentValidation;
using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Admin.AppService.Enums;
using SpaceReserve.Admin.Utility.Resources;

namespace SpaceReserve.Admin.AppService.Validators;

public class UserBookingHistorySortValidator : AbstractValidator<UserBookingHistorySortDto>
{
    [Obsolete]
    public UserBookingHistorySortValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Search)
            .Matches("^[a-zA-Z ]*$")
            .WithMessage("Name can only contain letters and spaces.");

        RuleFor(x => x.Sort)
            .Must(x => x == Convert.ToInt32(BookingFilter.All) || x==Convert.ToInt32( BookingFilter.Past) || x==Convert.ToInt32(BookingFilter.Upcoming))
            .WithMessage("Sort order must be 0,5,6");
        

    }
}
