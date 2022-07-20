namespace bot.Constants;

public static class StringConstants
{
    public static Dictionary<string, string> LanguageNames => new()
    {
        { "uz-Uz", "O'zbekcha" },
        { "ru-Ru", "Русский" },
        { "en-Us", "English" },
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
        {"office-msword", "Microsoft Word" },
        {"office-msexcel", "Microsoft Excel"},
        {"office-mspower","Microsoft Power Point" },
        {"office-adobe","Adobe Acrobat" },
        { "menu", "Ortga 🔙"},
    };

}