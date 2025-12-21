using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Contracts
{
    public interface ICheckUserProfileRepository
    {
        public Task<User> IsProfileModified(string subjectId);
        Task<SeatConfiguration?> GetSeatConfigurationByUserIdAsync(int userId);
    }
}