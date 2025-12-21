using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Admin.Infrastructure.Repositories
{
    public interface INotificationRepository
    {
        public Task AddNotifications(NotificationModel notificationModel);
        public Task<int> GetUserIdBySubjectId(string subjectId);
        public Task<string?> GetEmailByUserId(int userId);

    }
}