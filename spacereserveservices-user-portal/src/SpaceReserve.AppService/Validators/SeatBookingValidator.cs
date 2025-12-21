using FluentValidation;
using SpaceReserve.AppService.DTOs;

namespace SpaceReserve.AppService.Validators;

public class SeatBookingValidator:AbstractValidator<BookingDto>
{
    public SeatBookingValidator()
    {
        RuleFor(x => x.SeatId)
            .NotEmpty()
            .WithMessage("Seat ID is required.")
            .Must(seatId =>seatId > 0)
            .WithMessage("Seat ID must be an integer value.");
        
        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Please enter reason")
            .MaximumLength(200)
            .WithMessage("Exceeded maximum character limit of 200.");
        
        RuleFor(x => x.RequestDateTime)
            .NotEmpty()
            .WithMessage("Booking date is required.")
            .Must(BeWithinDateRange)
            .WithMessage("Booking date must be at least two days from today and no more than 90 days ahead.")
            .Must(RequestDateTime =>RequestDateTime.DayOfWeek != DayOfWeek.Saturday && RequestDateTime.DayOfWeek != DayOfWeek.Sunday)
            .WithMessage("Date must be weekday(Monday to Friday).");
    }
            
    private bool BeWithinDateRange(DateOnly bookingDate)
    {
        var minDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        var maxDate = DateOnly.FromDateTime(DateTime.Today.AddDays(91));
        return bookingDate >= minDate && bookingDate <= maxDate;
    }
    
}
