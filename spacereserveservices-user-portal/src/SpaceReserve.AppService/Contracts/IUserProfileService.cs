using Microsoft.AspNetCore.Http;
using SpaceReserve.AppService.DTOs;

namespace SpaceReserve.AppService.Contracts;

public interface IUserProfileService
{
    Task<UserDto?> GetUserProfileBySubjectIdAsync(string subjectId);
    Task<bool?> UpdateUserProfileBySubjectIdAsync(string subjectId, UpdateProfileRequestDto updateProfileRequestDto);
}
