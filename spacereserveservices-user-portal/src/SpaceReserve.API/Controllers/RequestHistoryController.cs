using System.Reflection;
using FluentValidation;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SpaceReserve.API.Attributes;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Utility;

namespace SpaceReserve.API.Controllers;

[ApiController]
[Route("api/deskbook/users/")]
public class RequestHistoryController : BaseApiController
{
    private readonly IRequestHistoryService _requestHistoryService;
    private readonly IValidator<RequestHistoryStatusValidatorDto> _validator;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(RequestHistoryController));

    public RequestHistoryController(IRequestHistoryService requestHistoryService, IValidator<RequestHistoryStatusValidatorDto> validator)
    {
        _requestHistoryService = requestHistoryService;
        _validator = validator;
    }
    [HttpPut("request-history/{requestId}/action")]
    [Authorized("User")]
    public async Task<IActionResult> UpdateUserRequestStatus(int requestId, [FromBody] RequestHistoryStatusDto requestHistoryStatusDto)
    {
        try
        {
            var requestHistoryStatusValidator = new RequestHistoryStatusValidatorDto
            {
                RequestId = requestId,
                Action = requestHistoryStatusDto.Action
            };
            var validationResult = await _validator.ValidateAsync(requestHistoryStatusValidator);
            if (!validationResult.IsValid)
            {
                _logger.Warn("Validation failed for seat booking request.");
                return BadRequestError(string.Join(", ", validationResult.Errors.Select(e => $"{e.ErrorMessage}")));
            }
            
            if (SubjectId == null || SubjectId == string.Empty)
            {
                _logger.Error("Subject ID is null or empty.");
                return BadRequestError("User not found.");
            }
            var result = await _requestHistoryService.UpdateUserRequestStatusAsync(requestId, requestHistoryStatusDto.Action, SubjectId);

            _logger.Info($"Request status updated successfully for request ID {requestId}.");
            return Ok(result);

        }
        catch (ArgumentException ex)
        {
            _logger.Error($"An error occurred while updating request status: {ex.Message}", ex);
            return BadRequestError("Failed to update request status. " + ex.Message);
        }
        catch (Exception ex)
        {
            _logger.Error($"An unexpected error occurred: {ex.Message}", ex);
            return Error("Internal server error.");
        }
    }
    [HttpGet("request-history/requeststatus")]
    [Authorized("User")]
    public async Task<IActionResult> GetRequestStatus()
    {
        try
        {
            var statuses = await _requestHistoryService.GetRequestStatusesAsync();
            if (statuses == null || !statuses.Any())
            {
                return NotFound(new { message = "No request statuses found." });
            }
            var filteredStatuses = statuses.Select(s => new
            {
                s.BookingStatusId,
                s.BookingStatus
            });

            _logger.Info("Request statuses fetched successfully.");
            return Ok(filteredStatuses);
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while fetching request statuses.", ex);
            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }

    [HttpGet("request-history")]
    [Authorized("User")]
    public async Task<IActionResult> GetAllRequestHistory(int sort, int pageNo, int pageSize)
    {
        try
        {
            _logger.Info("Fetching user from context.");
            var user = await _requestHistoryService.GetBySubjectIdAsync(SubjectId);

            if (user == null)
            {
                _logger.Warn($"User with SubjectID {SubjectId} not found.");
                return Error("Something went wrong. while fetching the request history.");
            }
            if (user.ModeOfWorkId == 2)
            {
                //work from home user
                return Ok();
            }
            if (user.ModeOfWorkId == 3)
            {
                //Regular user
                return Ok();
            }

            //hybrid user
            var userSeatId = await _requestHistoryService.GetUserSeatID(user.UserId);
            if (userSeatId == 0)
            {
                _logger.Warn($"User with ID {user.UserId} has no seat ID.");
                return NotFound("No Request Found");
            }

            if (sort >= 1 && sort <= 4)
            {
                _logger.Info($"Fetching request history for user with ID {user.UserId} and Seat ID {userSeatId} with sort {sort}.");
                var filteredRequestHistory = await _requestHistoryService.GetAllRequestHistoryByStatus(userSeatId, sort, pageNo, pageSize);
                return Ok(filteredRequestHistory);
            }

            if (sort < 0 || sort > 4)
            {
                return BadRequest("Invalid sort value. Please provide a valid sort value between 1 and 4.");
            }

            _logger.Info($"Fetching request history for user with ID {user.UserId} and Seat ID {userSeatId}.");
            var requestHistory = await _requestHistoryService.GetAllRequestHistory(userSeatId, pageNo, pageSize);
            return Ok(requestHistory);
        }
        catch (Exception ex)
        {
            _logger.Error("Error while fetching the request history.", ex);
            return Error("Internal server error. Please try again later." + ex.Message);
        }
    }
}

