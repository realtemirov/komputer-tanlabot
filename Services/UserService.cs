using bot.Data;
using bot.Entity;
using Microsoft.EntityFrameworkCore;

namespace bot.Services;

public class UserService
{

    //bazani classga ulangan propertysi
    private readonly BotDbContext _context;


    //konstruktor yordamida bazani classga ulangan propertysi qo'shish
    public UserService(BotDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    //add user
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

    //get user from db
    public async Task<User?> GetUserAsync(long? userId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(_context.Users);

        return await _context.Users.FindAsync(userId);
    }


    //userni languagesini update qilish
    public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateLanguageCodeAsync(long? userId, string? languageCode)
    {
        ArgumentNullException.ThrowIfNull(languageCode);

        var user = await GetUserAsync(userId);

        if(user is null)
        {
            return (false, "User not found");
        }

        user.LanguageCode = languageCode;
        _context?.Users?.Update(user);
        await _context.SaveChangesAsync();

        return (true, null);
    }


    //get user's language code from db
    public async Task<string?> GetLanguageCodeAsync(long? userId)
    {
        var user = await GetUserAsync(userId);

        return user?.LanguageCode;
    }

    //user bazada bor yoqligini tekshiradi
    public async Task<bool> Exists(long userId)
        => await _context.Users.AnyAsync(u => u.UserId == userId);
}