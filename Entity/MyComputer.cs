namespace bot.Entity;

public class MyComputer
{
    public Guid Id {get;set;}

    public long UserId {get;set;}

    public Guid? ComputerId {get;set;}

    public string? Link {get;set;}

    public DateTimeOffset CreatedAt {get;set;}

}