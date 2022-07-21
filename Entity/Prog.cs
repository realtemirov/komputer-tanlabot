namespace bot.Entity;

public class Prog
{
    public Guid Id { get; set; }

    public string? Query { get; set; }

    public string? Name { get; set; }

    public EProgType ProgType { get; set; }

    public double Point { get; set; }
    
}