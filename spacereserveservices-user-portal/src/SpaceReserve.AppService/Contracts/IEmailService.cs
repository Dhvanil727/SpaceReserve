using SpaceReserve.AppService.DTOs;

namespace SpaceReserve.AppService.Services
{
    public interface IEmailService
    {
        public Task SendEmailAsync(EmailDto emailDto);
    }
}