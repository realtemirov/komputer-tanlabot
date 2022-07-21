using bot.Data;
using bot.Entity;
using Microsoft.EntityFrameworkCore;

namespace bot.Services;

public class ChosenAppService
{
    private readonly BotDbContext _context;


    //konstruktor yordamida bazani classga ulangan propertysi qo'shish
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
            var result = await _context.ChosenApps.AddAsync(chosenApp);
            await _context.SaveChangesAsync();

            return (true, "Add chosenApp");
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }

    
    //get prog from db
    public async Task<ChosenApp> GetChosenAppAsync(Guid id)
    {
        //ArgumentNullException.ThrowIfNull(id);
        //ArgumentNullException.ThrowIfNull(_context.Prog);

        return await _context.ChosenApps.FindAsync(id);
    }

    //prog bazada bor yoqligini tekshiradi
    public async Task<bool> Exists(Guid id)
        => await _context.ChosenApps.AnyAsync(p => p.Id == id);
}