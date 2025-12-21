using Microsoft.EntityFrameworkCore;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetBySubjectIdAsync(string subjectId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.SubjectId == subjectId);
    }

    public async Task<List<string>> GetAllEmails()
    {
        return await _context.Users.Select(u => u.Email).ToListAsync();
    }


}
