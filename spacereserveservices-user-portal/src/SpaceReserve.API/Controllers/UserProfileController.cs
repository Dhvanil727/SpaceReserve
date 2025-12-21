using System.Reflection;
using FluentValidation;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceReserve.API.Attributes;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Utility;
using SpaceReserve.Utility.Resources;

namespace SpaceReserve.API.Controllers;

[ApiController]
[Route("api/deskbook")]
public class UserProfileController : BaseApiController
{
    private readonly IUserProfileService _userProfileService;
    private readonly ICheckUserProfileService _checkUserProfileService;
    private readonly IValidator<UpdateProfileRequestDto> _updateProfileRequestDtoValidator;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(UserProfileController));

    [Obsolete]
    public UserProfileController(IUserProfileService userProfileService, IValidator<UpdateProfileRequestDto> updateProfileRequestDtoValidator , ICheckUserProfileService checkUserProfile)
    {
        _userProfileService = userProfileService;
        _checkUserProfileService = checkUserProfile;
        _updateProfileRequestDtoValidator = updateProfileRequestDtoValidator;
        _logger.Info("UserProfileController initialized.");
    }

    [HttpGet("user-profile")]
    [Authorized(["User","Admin"])]
    public async Task<IActionResult> GetUserProfile()
    {
        try
        {
            var userProfile = await _userProfileService.GetUserProfileBySubjectIdAsync(SubjectId);
            if (userProfile == null)
            {
                return NotFound(CommonResource.UserNotFound);
            }
            return Ok(userProfile);
        }
        catch (Exception e)
        {
            _logger.Error("An unexpected error occurred while processing the request."+e.InnerException+e.StackTrace);
            return Error( CommonResource.ServerError+ e.Message);
        }
    }

    [HttpPut("user-profile")]
    [Authorized(["User","Admin"])]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateProfileRequestDto userProfileDto)
    {
        if (SubjectId == null || SubjectId == string.Empty)
        {
            _logger.Error("Subject ID is null or empty.");
            return BadRequestError("User not found.");
        }
        try
        {
            var validationResult = await _updateProfileRequestDtoValidator.ValidateAsync(userProfileDto);
            if (!validationResult.IsValid)
            {
                _logger.Warn("Validation failed for updation of user profile.");
                return BadRequestError(string.Join(", ", validationResult.Errors.Select((e) => $"{e.ErrorMessage}")));
            }
            var userDto = await _userProfileService.UpdateUserProfileBySubjectIdAsync(SubjectId, userProfileDto);
            return Ok(userDto);
        }
        catch (DbUpdateException e)
        {
            _logger.Error("SQL error occurred while processing the request.", e);
            return Error(e.InnerException?.Message ?? "An unknown SQL error occurred.");
        }
        catch (ArgumentException ex)
        {
            _logger.Error(ex.Message);
            return BadRequestError(ex.Message);
        }
        catch (Exception e)
        {
            _logger.Error("An unexpected error occurred while processing the request."+e.InnerException+e.StackTrace);
            return Error("Internal server error. Please try again later." + e.Message);
        }
    }

    [HttpGet("user-profile/isUpdated")]
    [Authorized(["User","Admin"])]
    public async Task<IActionResult> CheckUserProfile()
    {
        if (SubjectId == null || SubjectId == string.Empty)
        {
            _logger.Error("Subject ID is null or empty.");
            return BadRequestError("User not found.");
        }
        try
        {
            var userProfile = await _checkUserProfileService.IsProfileModified(SubjectId);
            return Ok(userProfile);
        }
        catch (Exception e)
        {
            _logger.Error("An unexpected error occurred while processing the request."+e.InnerException+e.StackTrace);
            return Error("Internal server error. Please try again later." + e.Message);
        }
    }
}
