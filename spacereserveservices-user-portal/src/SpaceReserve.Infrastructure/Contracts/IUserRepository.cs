using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Contracts;

public interface IUserRepository
{
    Task<User?> GetBySubjectIdAsync(string SubjectId);
    Task AddUserAsync(User user);
    Task<List<string>> GetAllEmails();
}
