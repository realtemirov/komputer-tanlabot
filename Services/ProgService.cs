using bot.Data;
using bot.Entity;
using Microsoft.EntityFrameworkCore;

namespace bot.Services;

public class ProgService
{
    private readonly BotDbContext _context;

    public ProgService(BotDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> AddProgAsync(Prog prog)
    {
            if(await Exists(prog.Query))
            {
                if(_context.Progs.Count(x => x.Query == prog.Query)> 1)
                {
                    _context.Remove(_context.Progs.FirstOrDefault(x => x.Query == prog.Query));
                    _context.SaveChanges();
                }
                return (false, "Prog exists");
                
            }
            try
            {
                var result = await _context.Progs.AddAsync(prog);
                await _context.SaveChangesAsync();
                return (true, "Add prog");
            }
            catch(Exception e)
            {
                return (false, e.Message);
            }
    }

    
    public async Task<Prog> GetProgAsync(string query)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(_context.Progs);
        return await _context.Progs.FindAsync(query);
    }

    public async Task<bool> Exists(string query)
        => await _context.Progs.AnyAsync(p => p.Query == query);
}