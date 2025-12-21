using Microsoft.Extensions.DependencyInjection;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.Services;

namespace SpaceReserve.AppService.Extensions;

public static class AppServiceExtension
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ISeatBookingService, SeatBookingService>();
        services.AddTransient<IBookingHistoryService, BookingHistoryService>();
        services.AddTransient<IBackgroundTaskService, BackgroundTaskService>();
        services.AddTransient<IReferenceService, ReferenceService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IUserProfileService, UserProfileService>();
        services.AddTransient<ICheckUserProfileService, CheckUserProfileService>();
        services.AddTransient<IRequestHistoryService, RequestHistoryService>();
    }
}
