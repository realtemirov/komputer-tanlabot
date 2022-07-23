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

    public async Task<List<Prog>> GetAllProgsAsync()
    {
        return  _context.Progs.ToList();
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> AddProgAsync(Prog prog)
    {
            if(await Exists(prog.Query))
            {
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

    
    public async Task<Prog?> GetProgAsync(string query)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(_context.Progs);
        var prog = _context.Progs.FirstOrDefaultAsync(p => p.Query == query).Result;
        return prog;
    }

    public async Task<bool> Exists(string query)
        => await _context.Progs.AnyAsync(p => p.Query == query);

    
}