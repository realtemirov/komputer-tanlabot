namespace bot.Constants;

public static class StringConstants
{
    public static Dictionary<string, string> LanguageNames => new()
    {
        { "uz-Uz", "O'zbekcha" },
        { "ru-Ru", "Русский" },
        { "en-Us", "English" },
    };

    public static Dictionary<string, string> Menu => new()
    {
        {"computer", "Kompyuter tanlash"},
        {"my-computers", "Mening komputerlarim"},
        {"settings", "Sozlamalar"},
        {"about-us", "Biz haqimizda"},
    };
    public static Dictionary<string,string> Programers => new()
    {
        { "office", "Offise dasturlari" },
        { "grafik", "Grafik dizayn" },
        { "video", "Video Editor’lar" },
        { "muhitlar", "Dasturlash muhitlari" },
    };

    public static Dictionary<string, string> OfficePrograms => new()
    {
        {"prog-office-msword", "Microsoft Word" },
        {"office-msexcel", "Microsoft Excel"},
        {"office-mspower","Microsoft Power Point" },
        {"office-adobe","Adobe Acrobat" },
        { "menu", "Ortga 🔙"},
    };

    public static Dictionary<string, string> ProgsResxToDictionary(string[] progs)
    {
        var result = new Dictionary<string, string>();
        result.Add("office", progs[0]);
        result.Add("graph", progs[1]);
        result.Add("videomaker", progs[2]);
        result.Add("ide", progs[3]);
        result.Add("browser", progs[4]);
        result.Add("social", progs[5]);
        result.Add("game", progs[6]);
        return result;
    }

    public static Dictionary<string, string> MenuResxToDictionary(string[] menu)
    {
        var result = new Dictionary<string, string>();
        result.Add("computer", menu[0]);
        result.Add("my-computers", menu[1]);
        result.Add("settings", menu[2]);
        result.Add("about-us", menu[3]);
        return result;
    }
}