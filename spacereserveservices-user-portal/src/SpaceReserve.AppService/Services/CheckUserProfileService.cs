using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Utility.Enum;
using SpaceReserve.Utility.Resources;

namespace SpaceReserve.AppService.Services
{
    public class CheckUserProfileService : ICheckUserProfileService
    {
        private readonly ICheckUserProfileRepository _checkUserProfileRepository;
        public CheckUserProfileService(ICheckUserProfileRepository checkUserProfileRepository)
        {
            _checkUserProfileRepository = checkUserProfileRepository;
        }
        public async Task<CheckProfileDto> IsProfileModified(string subjectId)
        {
            var userProfile = await _checkUserProfileRepository.IsProfileModified(subjectId);
            var seatConfiguration = await _checkUserProfileRepository.GetSeatConfigurationByUserIdAsync(userProfile.UserId);
            if (userProfile.IsAdmin)
            {
                return new CheckProfileDto
                {
                    ProfilePictureFileString = userProfile.ProfileImage,
                    Active = userProfile.IsActive,
                    Updated = true
                };
            }
            if (userProfile.DeletedDate == null && userProfile.DesignationId == null && userProfile.CityId == null && userProfile.ModeOfWorkId == null)
            {
                return new CheckProfileDto
                {
                    ProfilePictureFileString = null,
                    Active = userProfile.IsActive,
                    Updated = false
                };
            }
            else if (seatConfiguration == null && userProfile.ModeOfWorkId != (byte)ModeOfWork.WorkFromHome)
            {
                return new CheckProfileDto
                {
                    ProfilePictureFileString = null,
                    Active = userProfile.IsActive,
                    Updated = false
                };
            }
            else
            {
                return new CheckProfileDto
                {
                    ProfilePictureFileString = userProfile.ProfileImage,
                    Active = userProfile.IsActive,
                    Updated = true
                };
            }
        }
    }
}