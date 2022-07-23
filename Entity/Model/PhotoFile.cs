namespace bot.Entity.Model;

public class Result
{
    public string file_id { get; set; }
    public string file_unique_id { get; set; }
    public int file_size { get; set; }
    public string file_path { get; set; }
}

public class PhotoFile
{
    public bool ok { get; set; }
    public Result result { get; set; }
}