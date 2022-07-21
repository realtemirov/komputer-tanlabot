using bot.Data;
using bot.Entity;
using Microsoft.EntityFrameworkCore;

namespace bot.Services;

public class UserService
{
    private readonly BotDbContext _context;

    public UserService(BotDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> AddUserAsync(User user)
    {
        if(await Exists(user.UserId))
            return (false, "User exists");
            
        try
        {
            var result = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            
            return (true, null);
        }
        catch(Exception e)
        {
            return (false, e.Message);
        }
    }

    public async Task<User?> GetUserAsync(long? userId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(_context.Users);
        var userr = _context.Users.FindAsync(userId).Result;
        return userr;
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateLanguageCodeAsync(long? userId, string? languageCode)
    {
        ArgumentNullException.ThrowIfNull(languageCode);

        var user = GetUserAsync(userId).Result;

        if(user is null)
        {
            return (false, "User not found");
        }

        user.LanguageCode = languageCode;
        _context?.Users?.Update(user);
        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<string?> GetLanguageCodeAsync(long? userId)
    {
        var user = GetUserAsync(userId).Result;
        
        return user?.LanguageCode;
    }

    public async Task<bool> Exists(long userId)
        => await _context.Users.AnyAsync(u => u.UserId == userId);
}