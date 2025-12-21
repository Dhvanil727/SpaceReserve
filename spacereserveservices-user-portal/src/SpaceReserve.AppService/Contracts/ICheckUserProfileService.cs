using SpaceReserve.AppService.DTOs;

namespace SpaceReserve.AppService.Contracts
{
    public interface ICheckUserProfileService
    {
        public Task<CheckProfileDto> IsProfileModified(string subjectId);
    }
}