using bot.Data;
using bot.Entity;
using Microsoft.EntityFrameworkCore;

namespace bot.Services;

public class ChosenAppService
{
    private readonly BotDbContext _context;

    public ChosenAppService(BotDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }


    public async Task<List<ChosenApp>> GetAllChosenAppAsync()
    {
        return  _context.ChosenApps.ToList();
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> AddChosenAppAsync(ChosenApp chosenApp)
    {
        if (await Exists(chosenApp.ProgId, chosenApp.UserId))
            return (false, "ChosenApp exists");

        try
        {
            var result = await _context.ChosenApps.AddAsync(chosenApp);
            await _context.SaveChangesAsync();

            return (true, "Add chosenApp");
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }

    public async Task DeleteChosenAppAsync(long userId)
    {
        var app = _context.ChosenApps.Where(x => x.UserId == userId);

        foreach (var item in app)
        {
            _context.ChosenApps.Remove(item);
        }
        await _context.SaveChangesAsync();
    }
    
    public async Task<ChosenApp> GetChosenAppAsync(Guid id)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(_context.ChosenApps);

        return await _context.ChosenApps.FindAsync(id);
    }

    public async Task<bool> Exists(Guid? id, long userId)
        => await _context.ChosenApps.Where(u => u.UserId == userId && u.ProgId == id).AnyAsync();
}