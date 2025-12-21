using SpaceReserve.AppService.DTOs;

namespace SpaceReserve.AppService.Contracts;

public interface IUserService
{
    Task FindOrCreateUserAsync(LoginDto loginDto);
}
