using Microsoft.EntityFrameworkCore;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;
        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddNotifications(NotificationModel notificationModel)
        {
            await _context.Notifications.AddAsync(notificationModel);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUserIdBySubjectId(string subjectId)
        {
            return await _context.Users
                .Where(u => u.SubjectId == subjectId)
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();
        }

        public async Task<string?> GetEmailByUserId(int userId)
        {
            return await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();
        }
    }
}