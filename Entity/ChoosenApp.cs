namespace bot.Entity;

public class ChosenApp
{
    public Guid Id { get; set; }

    public long UserId { get; set; }

    public Guid? ProgId { get; set; }

    public DateTimeOffset ChosenTime { get; set; }
    
}