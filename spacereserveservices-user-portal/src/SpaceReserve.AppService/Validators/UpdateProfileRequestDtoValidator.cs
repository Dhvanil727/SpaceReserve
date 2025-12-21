using System.Data;
using FluentValidation;
using SpaceReserve.AppService.DTOs;

namespace SpaceReserve.AppService.Validators;

public class UpdateProfileRequestDtoValidator : AbstractValidator<UpdateProfileRequestDto>
{
    [Obsolete]
    public UpdateProfileRequestDtoValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.ProfilePictureFileString)
            .Must(x => IsValidBase64ImageFormat(x))
            .WithMessage("Upload valid image, only .jpg, .jpeg and .png are allowed");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches("^[0-9]*$")
            .WithMessage("Please enter a numeric value only.")
            .Length(10)
            .WithMessage("10-digit number is required")
            .Must(x => !x.Contains(" "))
            .WithMessage("Phone number must not include white space.");

        RuleFor(x => x.Designation)
            .NotEmpty()
            .WithMessage("Designation is required.")
            .Must(x => x >= 1 && x <= 10)
            .WithMessage("Designation must be between 1 and 10");

        RuleFor(x => x.ModeOfWork)
            .NotEmpty()
            .WithMessage("Mode of work is required.")
            .Must(x => x >= 1 && x <= 3)
            .WithMessage("Mode of work must be between 1 and 3");

        RuleFor(x => x.WorkingDays)
            .NotEmpty()
            .When(x => x.ModeOfWork == 1)
            .WithMessage("Working Days are required for the Hybrid employees");

        RuleFor(x => x.WorkingDays)
            .Must(workingDays => workingDays.All(day => day >= 1 && day <= 5))
            .WithMessage("Working days must be between 1 and 5");

        RuleFor(x => x.WorkingDays)
            .Must(workingDays => workingDays == null || !workingDays.Any())
            .When(x => x.ModeOfWork == 3 || x.ModeOfWork == 2)
            .WithMessage("Regular and Work From users cannot enter working days.");

        RuleFor(x => x.City)
            .NotEmpty()
            .When(x => x.ModeOfWork != 2)
            .WithMessage("City is required")
            .Must(x => x == 1 || x == 2)
            .When(x => x.ModeOfWork != 2)
            .WithMessage("City must be either 1 (Valsad) or 2 (Surat)");

        RuleFor(x => x.Floor)
            .NotEmpty()
            .When(x => x.ModeOfWork != 2)
            .WithMessage("Floor is required")
            .Must(x => x >= 1 && x <= 8)
            .When(x => x.ModeOfWork != 2)
            .WithMessage("Floor must be between 1 and 8");

        RuleFor(x => x.Column)
            .NotEmpty()
            .When(x => x.ModeOfWork != 2)
            .WithMessage("Column is required")
            .Must(x => x >= 1 && x <= 80)
            .When(x => x.ModeOfWork != 2)
            .WithMessage("Column must be between 1 and 80");

        RuleFor(x => x.WorkingDays)
            .Must(workingDays => workingDays != null && workingDays.Count() <= 4)
            .When(x => x.ModeOfWork == 1)
            .WithMessage("Hybrid users can enter a maximum of 4 working days.");
    }

    public static bool IsValidBase64ImageFormat(string imageString)
    {
        if (string.IsNullOrWhiteSpace(imageString))
        {
            return true;
        }

        if (!(imageString.StartsWith("data:image/png;base64")
        || imageString.StartsWith("data:image/jpg;base64")
        || imageString.StartsWith("data:image/jpeg;base64")))

        {
            return false;
        }

        return true;
    }

    public static bool IsValidBase64Size(string imageString)
    {
        const int maxFileSizeBytes = 2 * 1024 * 1024;

        try
        {
            var base64Data = imageString.Substring(imageString.IndexOf(",") + 1);
            byte[] imageBytes = Convert.FromBase64String(imageString);
            if (imageBytes.Length > maxFileSizeBytes)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
