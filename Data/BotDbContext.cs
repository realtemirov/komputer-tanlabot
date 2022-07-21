using bot.Entity;
using Microsoft.EntityFrameworkCore;

namespace bot.Data;

public class BotDbContext : DbContext
{
    public DbSet<User>? Users { get; set; }

    public DbSet<Prog>? Progs { get; set; }

    public DbSet<ChosenApp>? ChoosenApps { get; set; }

    public DbSet<Kompyuter>? Kompyuters {get;set;}

     public BotDbContext(DbContextOptions<BotDbContext> options)
        : base(options) { }
}
