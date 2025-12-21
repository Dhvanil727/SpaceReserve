namespace SpaceReserve.AppService.Contracts;

public interface IBackgroundTaskService
{
   Task<bool> ProcessPendingBookings(); 
}
