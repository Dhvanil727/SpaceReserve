using System.Net;
using System.Reflection;
using log4net;
using SpaceReserve.Utility;

namespace SpaceReserve.Admin.API.MiddleWare;

public class GlobalExceptionHandler: BaseApiController
{
    private readonly RequestDelegate _next; 
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(GlobalExceptionHandler));
    public GlobalExceptionHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.Error($"An unexpected error occurred while processing the request {ex.Message}.");
            _logger.Error($"stack trace =  {ex.StackTrace}.");
            _logger.Error($"inner exception =   {ex.InnerException}.");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(Errorm(ex.Message));
        }
    }

}
