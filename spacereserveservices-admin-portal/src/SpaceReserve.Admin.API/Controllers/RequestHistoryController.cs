using System.Reflection;
using FluentValidation;
using log4net;
using Microsoft.AspNetCore.Mvc;
using SpaceReserve.Admin.API.Attributes;
using SpaceReserve.Admin.AppService.Contracts;
using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Utility;

namespace SpaceReserve.Admin.API.Controllers
{
    [ApiController]
    [Route("api/deskbook")]
    public class RequestHistoryController : BaseApiController
    {
        private readonly IRequestHistoryService _requestHistoryService;
        private readonly IValidator<RequestHistoryQueryParamDto> _requestHistoryQueryDtoValidator;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(RequestHistoryController));
        private readonly IValidator<RequestHistoryActionDtoValidator> _requestHistoryActionDtoValidator;
        public RequestHistoryController(IRequestHistoryService requestHistoryService, IValidator<RequestHistoryActionDtoValidator> requestHistoryActionDtoValidator, IValidator<RequestHistoryQueryParamDto> requestHistoryQueryDtoValidator)
        {
            _requestHistoryService = requestHistoryService;
            _requestHistoryActionDtoValidator = requestHistoryActionDtoValidator;
            _requestHistoryQueryDtoValidator = requestHistoryQueryDtoValidator;
        }

        [HttpPut("request-history/{requestId}/action")]
        [Authorized("Admin")]
        public async Task<IActionResult> UpdateUserRequestStatusAsync(int requestId, [FromBody] RequestHistoryActionDto requestHistoryActionDto)
        {
            try
            {
                var requestHistoryActionValidator = new RequestHistoryActionDtoValidator
                {
                    RequestId = requestId,
                    Action = requestHistoryActionDto.Action
                };

                var validationResult = await _requestHistoryActionDtoValidator.ValidateAsync(requestHistoryActionValidator);
                if (!validationResult.IsValid)
                {
                    _logger.Warn($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
                    return BadRequestError(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                }

                var result = await _requestHistoryService.UpdateUserRequestStatusAsync(requestId, requestHistoryActionDto.Action, SubjectId);
                if (result == true)
                {
                    _logger.Info($"Request with ID {requestId} status updated successfully.");
                    return Ok(result);
                }

                _logger.Warn($"Request with ID {requestId} not found or status update failed.");
                return NotFound("Request not found or status update failed.");

            }
            catch (ArgumentException ex)
            {
                _logger.Error("Error while updating" + ex.Message);
                return BadRequestError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating request status: {ex.Message}", ex);
                return Error("An error occurred while updating the request status." + ex.Message);
            }
        }

        [HttpGet("request-history")]
        [Authorized("Admin")]

        public async Task<IActionResult> GetRequestHistoryAsync([FromQuery] RequestHistoryQueryParamDto requestHistory)
        {
            try
            {
                var validationResult = await _requestHistoryQueryDtoValidator.ValidateAsync(requestHistory);
                if (!validationResult.IsValid)
                {
                    _logger.Warn("Validation failed for booking history query.");
                    return BadRequestError(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                }
                var result = await _requestHistoryService.GetRequestHistoryAsync(requestHistory.Sort, requestHistory.PageNo, requestHistory.PageSize);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.Error("Error occurred while getting request history." + e.Message + " " + e.StackTrace);
                return Error("An error occurred while getting request history." + e.Message + " " + e.StackTrace);
            }

        }

        [HttpGet("request-history/requeststatus")]
        [Authorized("Admin")]

        public async Task<IActionResult> GetRequestHistoryStatusDropdown()
        {
            try
            {
                var result = await _requestHistoryService.GetRequestStatusDropdwon();
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.Error("Error occurred while getting request history status dropdown values." + e.Message + " " + e.StackTrace);
                return Error("An error occurred while getting request history status dropdown values.");
            }
        }

        [HttpGet("request-history/{requestId}/action")]
        [Authorized(["Admin","User"])]

        public async Task<IActionResult> GetRequestHistoryByIdAsync(int requestId)
        {
            try
            {
                var result = await _requestHistoryService.GetRequestHistoryByIdAsync(requestId);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.Error("Error occurred while getting request history by ID." + e.Message + " " + e.StackTrace);
                return Error("An error occurred while getting request history by ID." + e.Message + " " + e.StackTrace);
            }
        }
    }
}