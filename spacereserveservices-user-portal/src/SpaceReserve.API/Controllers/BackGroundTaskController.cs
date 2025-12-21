using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpaceReserve.API.Attributes;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.Utility;

namespace SpaceReserve.API.Controllers;

[ApiController]
[Route("api/backgroundtask")]
public class BackGroundTaskController : BaseApiController
{
    private readonly IBackgroundTaskService _backGroundTaskService;
    public BackGroundTaskController(IBackgroundTaskService backGroundTaskService)
    {
        _backGroundTaskService = backGroundTaskService;
    }

    [HttpPost]
    [Authorized("Admin")]
    public  async Task<IActionResult> AutoApprove()
    {
        await _backGroundTaskService.ProcessPendingBookings();
        return Ok();
    }

}
