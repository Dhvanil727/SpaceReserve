using System.Reflection;
using log4net;
using Microsoft.AspNetCore.Mvc;
using SpaceReserve.Admin.API.Attributes;
using SpaceReserve.Admin.AppService.Contracts;
using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Admin.Utility.Resources;
using SpaceReserve.Utility;

namespace SpaceReserve.Admin.API.Controllers;

[ApiController]
[Route("api/deskbook")]

public class SeatConfigurationController : BaseApiController
{
    private readonly ISeatConfigurationService _seatConfigurationService;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(SeatConfigurationController));

    public SeatConfigurationController(ISeatConfigurationService seatConfigurationService)
    {
        _seatConfigurationService = seatConfigurationService;
    }

    [HttpGet("Cities/{cityId}/floors/{floorId}/seats")]
    [Authorized("Admin")]
    public async Task<IActionResult> GetSeatsByFloorIdAsync(byte cityId, byte floorId)
    {
        try
        {
            var result = await _seatConfigurationService.GetSeatsByFloorIdAsync(cityId, floorId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error("Error in GetAllSeats", ex);
            return Error(CommonResources.ServerError + ex.Message);
        }
    }

    [HttpPost("Seats")]
    [Authorized("Admin")]
    public async Task<IActionResult> AddSeatAsync(List<AddSeatRequestDto> addSeatRequestDto)
    {
        try
        {
            if (string.IsNullOrEmpty(SubjectId))
            {
                _logger.Error("User ID is null or empty.");
                return BadRequestError(CommonResources.UserNotFound);
            }
            var user = await _seatConfigurationService.GetUserByIdAsync(SubjectId);

            await _seatConfigurationService.AddSeatAsync(addSeatRequestDto, user!.UserId);
            _logger.Info("Seat Added Successfully.");
            return Ok(CommonResources.Addseat);

        }
        catch(ArgumentException e)
        {
            _logger.Error(e.Message);
            return BadRequestError(e.Message);
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while add seat.", ex);
            return Error(CommonResources.ServerError + ex.Message);
        }
    }
    
    [HttpPut("Seats")]
    [Authorized("Admin")]
    public async Task<IActionResult> UpdateSeatStatus([FromBody] SeatStatusUpdateDto seatStatusUpdateDto)
    {
        try
        {
            if (string.IsNullOrEmpty(SubjectId))
            {
                return Unauthorized(CommonResources.UserNotFound);
            }
            var result = await _seatConfigurationService.UpdateSeatStatusAsync(SubjectId, seatStatusUpdateDto);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.Error("Seat status update DTO is null.", ex);
            return BadRequestError(CommonResources.BadRequestError);
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while updating seat status.", ex);
            return Error(CommonResources.ServerError + ex.Message);
        }
    }
}



