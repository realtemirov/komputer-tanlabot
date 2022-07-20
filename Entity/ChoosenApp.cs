namespace bot.Entity;

public class ChoosenApp
{
    public Guid Id { get; set; }

    public long UserId { get; set; }

    public Guid ProgId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset LastInteractionAt { get; set; }
}