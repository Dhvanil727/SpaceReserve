using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpaceReserve.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using SpaceReserve.Admin.Infrastructure.Repositories;
using SpaceReserve.Admin.Infrastructure.Contracts;

namespace SpaceReserve.Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection")));
        services.AddScoped<IBookingHistoryRepository, BookingHistoryRepository>();
        services.AddScoped<IRequestHistoryRepository, RequestHistoryRepository>();
        services.AddScoped<IRegisteredUserRepository, RegisteredUserRepository>();
        services.AddScoped<ISeatConfigurationRepository, SeatConfigurationRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }
}
