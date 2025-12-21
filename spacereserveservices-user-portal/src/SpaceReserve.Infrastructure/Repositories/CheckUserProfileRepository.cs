using Microsoft.EntityFrameworkCore;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Repositories
{
    public class CheckUserProfileRepository : ICheckUserProfileRepository
    {
        private readonly AppDbContext _context;
        public CheckUserProfileRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<User> IsProfileModified(string subjectId)
        {
            var userProfile = await _context.Users
                    .Where(u => u.SubjectId == subjectId && u.DeletedDate == null)
                    .FirstOrDefaultAsync();
            return userProfile;
        }
        public async Task<SeatConfiguration?> GetSeatConfigurationByUserIdAsync(int userId)
        {
            return await _context.SeatConfigurations.FirstOrDefaultAsync(sc => sc.UserId == userId && sc.DeletedDate == null);
        }
    }
}