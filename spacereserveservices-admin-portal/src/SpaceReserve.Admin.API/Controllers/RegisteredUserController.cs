using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using log4net;
using Microsoft.AspNetCore.Mvc;
using SpaceReserve.Admin.API.Attributes;
using FluentValidation;
using SpaceReserve.Admin.AppService.Contracts;
using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Utility;
using SpaceReserve.Admin.Utility.Resources;

namespace SpaceReserve.Admin.API.Controllers;

[ApiController]
[Route("api/deskbook/users")]
public class RegisteredUserController : BaseApiController
{
    private readonly IRegisteredUserService _registeredUserService;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(RegisteredUserController));
    private readonly IValidator<UpdateUserStatusDto> _updateUserStatusDtoValidator;
    private readonly IValidator<UserBookingHistorySortDto> _userBookingHistorySortDto;
    public RegisteredUserController(IRegisteredUserService registeredUserService, IValidator<UpdateUserStatusDto> updateUserStatusDtoValidator,IValidator<UserBookingHistorySortDto> userBookingHistorySortDto)
    {
        _updateUserStatusDtoValidator = updateUserStatusDtoValidator;
        _registeredUserService = registeredUserService;
        _userBookingHistorySortDto = userBookingHistorySortDto;
    }

    [HttpGet("booking-history/{userId}")]
    [Authorize("Admin")]
    public async Task<IActionResult> GetAllBookingHistoryOfUserAsync(int userId, [FromQuery] UserBookingHistorySortDto userBookingHistorySortDto)
    {
         try
            {
                var validationResult = await _userBookingHistorySortDto.ValidateAsync(userBookingHistorySortDto);
                if (!validationResult.IsValid)
                {
                    _logger.Warn("Validation failed for booking history query.");
                    return BadRequestError(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                }
                var bookingHistory = await _registeredUserService.GetAllBookingHistoryOfUserAsync(userBookingHistorySortDto,userId);
                
                return Ok(bookingHistory);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
    }

    [HttpGet("profile/{userId}")]
    [Authorized("Admin")]

    public async Task<IActionResult> GetUserProfile(int userId)
    {
        try
        {
            var userProfile = await _registeredUserService.GetUserProfileBySubjectIdAsync(userId);
            if (userProfile == null)
            {
                return NotFound(CommonResources.UserNotFound);
            }
            return Ok(userProfile);
        }
        catch (Exception e)
        {
            _logger.Error("An unexpected error occurred while processing the request." + e.InnerException + e.StackTrace);
            return Error(CommonResources.ServerError + e.Message);
        }
    }

    [HttpGet]
    [Authorized("Admin")]
    public async Task<IActionResult> GetAllUsers(int pageNo, int pageSize)
    {
        try
        {
            var users = await _registeredUserService.GetAllUsersAsync(pageNo, pageSize);
            _logger.Info("All users displayed successfully.");
            return Ok(users);
        }
        catch (Exception e)
        {
            _logger.Error("An unexpected error occurred while processing the request.", e);
            return Error("Internal server error." + e.Message);
        }
    }

    [HttpPut("status")]
    [Authorized("Admin")]
    public async Task<IActionResult> UpdateUserStatusAsync([FromBody] List<UpdateUserStatusDto> updateUserStatusDto)
    {
        try
        {
            if (updateUserStatusDto == null || !updateUserStatusDto.Any())
            {
                throw new ArgumentException("Please enter the desired value properly");
            }
            var validationErrors = new List<string>();
            foreach (var dto in updateUserStatusDto)
            {
                var validationResult = await _updateUserStatusDtoValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    validationErrors.AddRange(validationResult.Errors.Select(e => $"UserId: {dto.UserId} - {e.ErrorMessage}"));
                }
            }
            if (validationErrors.Any())
            {
                _logger.Warn("Validation failed for Updating user status.");
                return BadRequestError(string.Join(", ", validationErrors));
            }

            var loggedInAdminSubjectId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
            var result = await _registeredUserService.UpdateUserStatusAsync(updateUserStatusDto, loggedInAdminSubjectId);
            _logger.Info($"User status updated successfully for multiple users");
            return Ok(result);
        }
        catch (ArgumentException e)
        {
            _logger.Error(e.Message);
            return BadRequestError(e.Message);
        }
        catch (KeyNotFoundException x)
        {
            _logger.Error(x.Message);
            return BadRequestError(x.Message);
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while updating user status.", ex);
            return Error("An unexpected error occurred." + ex.Message);
        }
    }
}
