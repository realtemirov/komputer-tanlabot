using ConvertApiDotNet;
using ConvertApiDotNet.Model;

namespace bot.Helpers;

public static class Convert
{
    public static async Task ConvertTxtToPdf(long userId)
    {       
        ConvertApi convertApi = new ConvertApi("BdqUk2GXnWRXdP78");
        ConvertApiResponse result = await convertApi.ConvertAsync("txt", "pdf", new[]
        {
            new ConvertApiFileParam($"{userId}.txt")
        });

        
        var fileInfo = await result.SaveFileAsync($"{userId}.pdf");
    }
}