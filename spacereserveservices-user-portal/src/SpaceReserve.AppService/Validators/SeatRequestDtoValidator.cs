using System.Globalization;
using FluentValidation;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Utility.Enum;

namespace SpaceReserve.AppService.Validators;

public class SeatRequestDtoValidator : AbstractValidator<SeatRequestDto>
{
    [Obsolete]
    public SeatRequestDtoValidator()
    {
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required.")
            .Must(date => DateOnly.Parse(date) >= DateOnly.FromDateTime(DateTime.Now.AddDays(2)) && DateOnly.Parse(date) <= DateOnly.FromDateTime(DateTime.Now.AddDays(91)))
            .WithMessage("Date must be between 2 days from today and 90 days from today.")
            .Must(date => DateOnly.Parse(date).DayOfWeek != DayOfWeek.Saturday && DateOnly.Parse(date).DayOfWeek != DayOfWeek.Sunday)
            .WithMessage("Date must be weekday(Monday to Friday).");

        RuleFor(x => x.CityId)
            .NotEmpty()
            .WithMessage("CityId is required.")
            .Must(cityId => cityId ==(byte)CityId.Valsad || cityId == (byte)CityId.Surat)
            .WithMessage("CityId must be either 1 or 2.");

        RuleFor(x => x.FloorId)
            .NotEmpty()
            .WithMessage("FloorId is required.")
            .Must(floorId => floorId == (byte)Floors.ValsadFirstFloor || floorId == (byte)Floors.ValsadSecondFloor || floorId == (byte)Floors.ValsadThirdFloor || floorId == (byte)Floors.ValsadFourthFloor)
            .When(x => x.CityId == (byte)CityId.Valsad)
            .WithMessage("FloorId must be one of the following: 1, 2, 3, or 4 for CityId 1 (Valsad).");

        RuleFor(x => x.FloorId)
            .NotEmpty()
            .WithMessage("FloorId is required.")
            .Must(floorId => floorId == (byte)Floors.SuratFirstFloor || floorId == (byte)Floors.SuratSecondFloor || floorId == (byte)Floors.SuratThirdFloor || floorId == (byte)Floors.SuratFourthFloor)
            .When(x => x.CityId == (byte)CityId.Surat)
            .WithMessage("FloorId must be one of the following: 5, 6, 7, or 8 for CityId 2 (Surat).");


    }
}
