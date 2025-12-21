using FluentValidation;
using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Admin.AppService.Enums;

namespace SpaceReserve.AppService.Validators;

public class BookingHistoryValidator : AbstractValidator<BookingHistoryQueryDto>
{
    [Obsolete]
    public BookingHistoryValidator()
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
