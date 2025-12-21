using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpaceReserve.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Repositories;

namespace SpaceReserve.Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISeatBookingRepository, SeatBookingRepository>();
        services.AddScoped<IBackgroundTaskRepository, BackgroundTaskRepository>();
        services.AddScoped<IBookingHistoryRepository, BookingHistoryRepository>();
        services.AddScoped<IReferenceRepository, ReferenceRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<ICheckUserProfileRepository, CheckUserProfileRepository>();
        services.AddScoped<IRequestHistoryRepository, RequestHistoryRepository>();
    }
}
