using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Admin.AppService.Contracts;

public interface IRegisteredUserService
{
    Task<List<UserBookingHistoryDto>> GetAllBookingHistoryOfUserAsync(UserBookingHistorySortDto userBookingHistorySortDto, int userId);
    Task<UserDto?> GetUserProfileBySubjectIdAsync(int userId);
    Task<IEnumerable<RegisteredUserDto>> GetAllUsersAsync(int pageNo, int pageSize);
    Task<string> UpdateUserStatusAsync(List<UpdateUserStatusDto> updateUserStatusDto, string adminSubjectId); 
    Task<User?> GetAdminBySubjectIdAsync(string adminSubjectId);
}
