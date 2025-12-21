namespace SpaceReserve.Utility;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

public class BaseApiController : ControllerBase
{
    public string subjectId = string.Empty;
    protected string SubjectId
    {
        get => subjectId = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
    }

    protected new IActionResult Ok()
    {
        return base.Ok(ServiceResponse.Ok());
    }

    protected IActionResult Ok<T>(T data)
    {
        return base.Ok(ServiceResponse.Ok(data, null));
    }

    protected IActionResult BadRequestError(string error)
    {
        return BadRequest(ServiceResponse.ErrorMessage(error));
    }
    protected IActionResult Error(string error)
    {
        return StatusCode(500, ServiceResponse.ErrorMessage(error));
    }

    protected IActionResult NotFound(string error)
    {
        return NotFound(ServiceResponse.NotFound(error));
    }

    protected string Errorm(string errorMsg) //For global error handling.
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Serialize(ServiceResponse.ErrorMessage(errorMsg), options);
    }

}

