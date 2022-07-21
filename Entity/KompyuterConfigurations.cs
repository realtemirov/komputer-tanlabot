using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bot.Entity;

public class KompyuterConfigurations : IEntityTypeConfiguration<Kompyuter>
{
    public void Configure(EntityTypeBuilder<Kompyuter> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.ModelName).HasMaxLength(255);

    }
}