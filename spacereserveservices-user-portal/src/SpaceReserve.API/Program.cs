using Hangfire;
using SpaceReserve.API.Extensions;
using SpaceReserve.API.MiddleWare;
using SpaceReserve.AppService.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#pragma warning disable CS0612 // Type or member is obsolete
builder.AddStartupServices();
#pragma warning restore CS0612 // Type or member is obsolete
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseMiddleware<GlobalExceptionHandler>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<IBackgroundTaskService>
    (
        recurringJobId: " AutoApprovePendingBookings",
        methodCall:  x => x.ProcessPendingBookings(),
        cronExpression: builder.Configuration["JobSchedule:BackgroundJob"],
        queue: "user-side"
    );

app.MapControllers();
app.Run();