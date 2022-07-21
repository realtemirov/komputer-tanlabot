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
                if(_context.Prog.Count(x => x.Query == prog.Query)> 1)
                {
                    _context.Remove(_context.Prog.FirstOrDefault(x => x.Query == prog.Query));
                    _context.SaveChanges();
                }
                return (false, "Prog exists");
                
            }
            try
            {
                var result = await _context.Prog.AddAsync(prog);
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
        ArgumentNullException.ThrowIfNull(_context.Prog);
        return await _context.Prog.FindAsync(query);
    }

    public async Task<bool> Exists(string query)
        => await _context.Prog.AnyAsync(p => p.Query == query);
}