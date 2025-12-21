using System.Globalization;
using System.Reflection;
using FluentValidation;
using log4net;
using Microsoft.AspNetCore.Mvc;
using SpaceReserve.API.Attributes;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Utility;
using SpaceReserve.Utility.Resources;

namespace SpaceReserve.API.Controllers;

[ApiController]
[Route("api/deskbook")]
public class SeatBookingController : BaseApiController
{
    private readonly ISeatBookingService _seatBookingService;
    private readonly IValidator<SeatRequestDto> _validator;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(SeatBookingController));
    private readonly IValidator<BookingDto> _bookingDtoValidator;

    public SeatBookingController(ISeatBookingService seatBookingService, IValidator<SeatRequestDto> validator, IValidator<BookingDto> bookingDtoValidator)
    {
        _seatBookingService = seatBookingService;
        _bookingDtoValidator = bookingDtoValidator;
        _validator = validator;
    }

    [HttpPost("request-seat")]
    [Authorized("User")]
    public async Task<IActionResult> BookSeat([FromBody] AddBookingDto addBookingDto)
    {
        try
        {
            DateOnly addBookingDate = DateOnly.ParseExact(addBookingDto.RequestDateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var bookingDto = new BookingDto
            {
                SeatId = addBookingDto.SeatId,
                Reason = addBookingDto.Reason,
                RequestDateTime = addBookingDate
            };

            var validationResult = await _bookingDtoValidator.ValidateAsync(bookingDto);
            if (!validationResult.IsValid)
            {
                _logger.Warn("Validation failed for seat booking.");
                return BadRequestError(string.Join(", ", validationResult.Errors.Select((e) => $"{e.ErrorMessage}")));
            }
            var booking = await _seatBookingService.BookSeat(bookingDto, SubjectId);
            if (booking != null)
            { _logger.Info("Seat booked successfully."); }

            return booking!.Status switch
            {
                ResultStatus.Success => Ok(booking.Value),
                ResultStatus.NotFound => NotFound(booking.Message!),
                ResultStatus.ErrorMessage => BadRequestError(booking.Message!),
                _ => throw new Exception("Unhandled result status")
            };
        }
        catch (FormatException)
        {
            _logger.Warn("Date is required in yyyy-MM-dd.");
            return BadRequestError("Invalid date format. Please use yyyy-MM-dd.");
        }

        catch (Exception e)
        {
            _logger.Error("An unexpected error occurred while processing the request.", e);
            return Error("Internal server error. Please try again later." + e.Message);
        }

    }

    [HttpGet("seat-view/get-seat-details")]
    [Authorized("User")]
    public async Task<IActionResult> GetSeatDetails([FromQuery] short seatId, [FromQuery] string date)
    {
        if (seatId <= 0)
        {
            _logger.Warn("SeatId must be greater than 0.");
            return BadRequestError("SeatId must be greater than 0.");
        }
        if (string.IsNullOrWhiteSpace(date) || !DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            _logger.Warn("Date is required and in yyyy-MM-dd format");
            return BadRequestError("Date is required and in yyyy-MM-dd format");
        }
        try
        {
            var result = await _seatBookingService.GetSeatDetailsAsync(seatId, parsedDate);

            return Ok(result);
        }
        catch (FormatException)
        {
            _logger.Warn("Date is required in yyyy-MM-dd.");
            return BadRequestError("Invalid date format. Please use yyyy-MM-dd.");
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while retrieving seat details.", ex);
            return Error("Internal server error. Please try again later." + ex.Message);
        }

    }

    [HttpGet("seat-view")]
    [Authorized("User")]
    public async Task<IActionResult> GetAllSeats([FromQuery] DateOnly date, [FromQuery] byte cityId, [FromQuery] byte floorId)
    {
        if (date == default)
        {
            throw new ArgumentException("Invalid date format. Expected format: yyyyMMdd");
        }
        try
        {
            var seatRequestDto = new SeatRequestDto
            {
                Date = date.ToString("yyyy-MM-dd"),
                CityId = cityId,
                FloorId = floorId
            };
            var validationResult = await _validator.ValidateAsync(seatRequestDto);
            if (!validationResult.IsValid)
            {
                _logger.Warn("Validation failed for seat booking request.");
                return BadRequestError(string.Join(", ", validationResult.Errors.Select(e => $"{e.ErrorMessage}")));
            }
            var result = await _seatBookingService.GetAllSeatsAsync(date, cityId, floorId);
            _logger.Info($"GetAllSeats called with date: {date}, cityId: {cityId}, floorId: {floorId}");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error("Error in GetAllSeats", ex);
            return StatusCode(500, "Internal server error");
        }
    }

}