using EventManagementDataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementDataAccessLayer;

public interface IEventManagementRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<User?> AuthenticateUserAsync(string email, string password);
}

public class EventManagementRepository : IEventManagementRepository
{
    private readonly EventManagementDbContext _context;

    public EventManagementRepository(EventManagementDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> AuthenticateUserAsync(string email, string password)
    {
        // Note: Compare hashed passwords in production instead of plain text
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
    }
}
