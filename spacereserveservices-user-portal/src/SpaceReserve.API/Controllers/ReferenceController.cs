using System.Linq;
using System.Reflection;
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
public class ReferenceController : BaseApiController
{
    private readonly IReferenceService _referenceService;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(ReferenceController));

    public ReferenceController(IReferenceService referenceService)
    {
        _referenceService = referenceService;
    }

    
    [HttpGet("designations")]
    [Authorized(["User","Admin"])]
    public async Task<IActionResult> GetDesignations()
    {
        try
        {
            var designations = await _referenceService.GetDesignationsAsync();

            _logger.Info("Fetched all designations successfully.");
            return Ok(designations);
        }
        catch (Exception ex)
        {
            _logger.Error("An unexpected error occurred while fetching designations.", ex);
            return Error("Internal server error. Please try again later." + ex.Message);
        }
    }
  
    [HttpGet("working-days")]
    [Authorized(["User","Admin"])]
    public async Task<IActionResult> GetWorkingDays()
    {
        try
        {
            var result = await _referenceService.GetWorkingDaysAsync();
            _logger.Info("Working Days successfully retrived.");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while retrieving working days.", ex);
         return Error("Internal server error. Please try again later." + ex.Message);
        }
    }

    [HttpGet("seats/{columnId}")]
    [Authorized(["User","Admin"])]
    public async Task<IActionResult> GetSeatsByColumnId(byte columnId)
    {
        try
        {
            var allSeats = await _referenceService.GetSeatsByColumnIdAsync(columnId);
            var seatsInConfig = await _referenceService.GetSeatsConfigurationByColumnIdAsync(SubjectId, columnId);
            var seatsInBooking = await _referenceService.GetSeatsIdFromBookingAsync();
            var underMaintenanceSeats =await _referenceService.GetUnderMaintainanceSeat();
            
            foreach (var seat in allSeats)
            {
                seat.Booked = seatsInBooking.Contains(seat.Id) || seatsInConfig.Contains(seat.Id) || underMaintenanceSeats.Contains(seat.Id);
            }

            _logger.Info("Seats retrieved successfully.");
            return Ok(allSeats);
        }
        catch (ArgumentException ex)
        {
            _logger.Error(ex.Message);
            return BadRequestError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while retrieving Seats.", ex);
            return Error(CommonResource.ServerError);
        }
    }

    [HttpGet("mode-of-works")]
    [Authorized(["User","Admin"])]
    public async Task<IActionResult> GetAllModeOfWorks()
    {
        try
        {
            var modes = await _referenceService.GetAllModeOfWorksAsync();
            _logger.Info("Mode of works retrieved successfully.");
            return Ok(modes);
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while retrieving mode of works.", ex);
            return Error(CommonResource.ServerError);
        }
    }
    
    [HttpGet("columns/{floorId}")]
    [Authorized(["User","Admin"])]
    public async Task<IActionResult> GetColumnsByFloorId(int floorId)
    {
        if (floorId <= 0)
        {
            _logger.Error("Invalid floorId provided.");
            return BadRequest("Invalid floorId.");
        }
        var columns = await _referenceService.GetColumnsByFloorIdAsync(floorId);
        if (columns.Count <= 0 || !columns.Any())
        {
            _logger.Error($"No columns found for floorId: {floorId}.");
            return NotFound("No columns found for the selected floor.");
        }
        return Ok(columns);
    }

    [HttpGet("cities")]
    [Authorized(["User","Admin"])]
    public async Task<IActionResult> GetAllCity()
    {
        try
        {
            var cities = await _referenceService.GetAllCityAsync();
            _logger.Info("city successfully retrived.");
            return Ok(cities);
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while retrieving city.", ex);
            return Error("Internal server error. Please try again later." + ex.Message);
        }
    }

    [HttpGet("floors/{cityId}")]
    [Authorized(["User","Admin"])]
    public async Task<IActionResult> GetFloorByCity(byte cityId)
    {
        try
        {
            var floors=await _referenceService.GetFloorByCityId(cityId);
            _logger.Info("Floor retrived successfully .");
            return Ok(floors);
        }
        catch (ArgumentException ex)
        {
            _logger.Error(ex.Message);
            return BadRequestError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while retrieving floor.", ex);
            return Error("Internal server error. Please try again later." + ex.Message);
        }
    }
}
