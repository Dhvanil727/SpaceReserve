using System.Reflection;
using FluentValidation;
using log4net;
using Microsoft.AspNetCore.Mvc;
using SpaceReserve.API.Attributes;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Utility;

namespace SpaceReserve.API.Controllers;

[ApiController]
[Route("api/deskbook")]
public class BookingHistoryController : BaseApiController
{
    private readonly IBookingHistoryService _bookingHistoryService;
    private readonly IValidator<BookingHistoryQueryDto> _bookingHistoryQueryDtoValidator;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(BookingHistoryController));

    public BookingHistoryController(IBookingHistoryService bookingHistoryService, IValidator<BookingHistoryQueryDto> bookingHistoryQueryDtoValidator)
    {
        _bookingHistoryService = bookingHistoryService;
        _bookingHistoryQueryDtoValidator = bookingHistoryQueryDtoValidator;
    }

    [HttpPut("cancel-request/{requestId}")]
    [Authorized("User")]
    public async Task<IActionResult> CancelBooking(int requestId)
    {
        try
        {
            var result = await _bookingHistoryService.CancelBookingAsync(requestId);
            if (result)
            {
                _logger.Info($"Booking with ID {requestId} cancelled successfully.");
                return Ok("Your seat has been cancelled successfully");
            }
            else
            {
                _logger.Warn($"Failed to cancel booking with ID {requestId}. It may not exist or is already cancelled.");
                return NotFound("Booking not found or already cancelled or Time limit exceeded.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Error while logging the cancellation request.", ex);
            return Error("Internal server error. Please try again later." + ex.Message);
        }
    }

    [HttpGet("booking-history")]
    [Authorized("User")]
    public async Task<IActionResult> GetAllBookingHistoriesAsync([FromQuery] BookingHistoryQueryDto booking)
    {
        try
        {
            var validationResult = await _bookingHistoryQueryDtoValidator.ValidateAsync(booking);
            if (!validationResult.IsValid)
            {
                _logger.Warn("Validation failed for booking history query.");
                return BadRequestError(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }
            var bookingHistory = await _bookingHistoryService.GetAllBookingHistoriesAsync((short)booking.Sort, booking.PageNo, booking.PageSize, SubjectId);
            return Ok(bookingHistory);
        }
        catch (Exception ex)
        {
            _logger.Error("Error while fetching booking history.", ex);
            return Error("Internal server error. Please try again later." + ex.Message);
        }
    }
}