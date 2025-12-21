using Microsoft.Extensions.DependencyInjection;
using SpaceReserve.Admin.AppService.Contracts;
using SpaceReserve.Admin.AppService.Services;
using SpaceReserve.Admin.Infrastructure.Repositories;
using SpaceReserve.AppService.Services;


public static class AppServiceExtension
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<ISeatConfigurationService, SeatConfigurationService>();
        services.AddScoped<IRequestHistoryService, RequestHistoryService>();
        services.AddScoped<IBookingHistoryService, BookingHistoryService>();
        services.AddScoped<IRegisteredUserService, RegisteredUserService>();
        services.AddScoped<IKeyCloakAppService, KeyCloakAppService>();
        services.AddAutoMapper(typeof(AutoMapperConfigurations));
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<INotificationRepository , NotificationRepository>();
    }
}
