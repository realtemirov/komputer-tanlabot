using bot.Entity;
using Microsoft.EntityFrameworkCore;

namespace bot.Data;

public class BotDbContext : DbContext
{
    public DbSet<User>? Users { get; set; }

    public BotDbContext(DbContextOptions<BotDbContext> options)
        : base(options) { }
}