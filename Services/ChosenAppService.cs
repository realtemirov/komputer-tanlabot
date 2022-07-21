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

    public async Task<(bool IsSuccess, string ErrorMessage)> AddChosenAppAsync(ChosenApp chosenApp)
    {
        if (await Exists(chosenApp.Id))
            return (false, "ChosenApp exists");

        try
        {
            var result = await _context.ChoosenApps.AddAsync(chosenApp);
            await _context.SaveChangesAsync();

            return (true, "Add chosenApp");
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }

    
    public async Task<ChosenApp> GetChosenAppAsync(Guid id)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(_context.ChoosenApps);

        return await _context.ChoosenApps.FindAsync(id);
    }

    public async Task<bool> Exists(Guid id)
        => await _context.ChoosenApps.AnyAsync(p => p.Id == id);
}