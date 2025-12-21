using System.Reflection;
using FluentValidation;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceReserve.API.Attributes;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Utility;
namespace SpaceReserve.API.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : BaseApiController
{
    private readonly IUserService _userService;

    private readonly IValidator<LoginDto> _loginDtoValidator;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(UserController));

    public UserController(IUserService userService, IValidator<LoginDto> loginDtoValidator )
    {
        _loginDtoValidator = loginDtoValidator;
        _userService = userService;
        _logger.Info("UserController initialized.");
  
    }

    [HttpPost("login")]
    [Authorized]
    public async Task<IActionResult> FindOrCreateUser([FromBody] LoginDto loginDto)
    {

        try
        {
            var validationResult = await _loginDtoValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                _logger.Warn("Validation failed for login.");
                return BadRequestError(string.Join(", ", validationResult.Errors.Select((e) => $"{e.ErrorMessage}")));
            }
            await _userService.FindOrCreateUserAsync(loginDto);
            _logger.Info("User found or created successfully.");
            return Ok();
        }
        catch(DbUpdateException e)
        {
            _logger.Error("SQL error occurred while processing the request.", e);
            return Error(e.InnerException?.Message ?? "An unknown SQL error occurred.");
        }
        catch (Exception e)
        {
            _logger.Error("An unexpected error occurred while processing the request.");
            return Error("Internal server error. Please try again later." + e.Message);
        }
    }
}









