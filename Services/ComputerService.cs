using bot.Data;
using bot.Entity;
using Microsoft.EntityFrameworkCore;

namespace bot.Services;

public class ComputerService
{
    private readonly BotDbContext _context;

    public ComputerService(BotDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task DeleteMyComps(long userId)
    {
        var comps = await _context.MyComputers.Where(c => c.UserId == userId).ToListAsync();
        foreach (var item in comps)
        {
            _context.MyComputers.Remove(item);
        }
        _context.SaveChangesAsync();

    }
    public async Task<List<Kompyuter>> GetAllCompsAsync()
    {
        return  _context.Kompyuters.ToList();
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> AddKompAsync(Kompyuter comp)
    {
            if(await Exists(comp.ModelName))
            {
                return (false, "Comp exists");   
            }
            try
            {
                var result = await _context.Kompyuters.AddAsync(comp);
                await _context.SaveChangesAsync();
                return (true, "Add Comp");
            }
            catch(Exception e)
            {
                return (false, e.Message);
            }
    }

    public async Task<List<Kompyuter>> GetAnyCompAsync(double grade)
    {
        return _context.Kompyuters.Where(x => x.Grade >= grade).ToList();
    }

    public async Task<Kompyuter?> GetCompAsync(Guid id)
    {
        var comp =  _context.Kompyuters.FirstOrDefault(c => c.Id == id);
        return comp;
    }

    public async Task<bool> Exists(string modelName)
        => await _context.Kompyuters.AnyAsync(p => p.ModelName == modelName);

    public async Task<List<MyComputer>> GetAllMyCompsAsync()
    {
        return  _context.MyComputers.ToList();
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> AddMyKompAsync(MyComputer comp)
    {
            if(await ExistsMyComp(comp.ComputerId))
            {
                return (false, "MyComp exists");   
            }
            try
            {
                var result = await _context.MyComputers.AddAsync(comp);
                await _context.SaveChangesAsync();
                return (true, "Add Comp");
            }
            catch(Exception e)
            {
                return (false, e.Message);
            }
    }

    public async Task<bool> ExistsMyComp(Guid? id)
        => await _context.MyComputers.AnyAsync(p => p.ComputerId == id);
}