using System.Reflection;
using FluentValidation;
using log4net;
using SpaceReserve.Admin.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using SpaceReserve.Admin.AppService.Contracts;
using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Utility;

namespace SpaceReserve.Admin.API.Controllers
{
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

        [HttpGet("booking-history")]
        [Authorized("Admin")]
        public async Task<IActionResult> GetBookingHistory([FromQuery] BookingHistoryQueryDto bookingHistoryQueryDto)
        {
            try
            {
                var validationResult = await _bookingHistoryQueryDtoValidator.ValidateAsync(bookingHistoryQueryDto);
                if (!validationResult.IsValid)
                {
                    _logger.Warn("Validation failed for booking history query.");
                    return BadRequestError(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                }
                var bookingHistory = await _bookingHistoryService.GetBookingHistoryAsync(bookingHistoryQueryDto);
                
                return Ok(bookingHistory);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

    }

}