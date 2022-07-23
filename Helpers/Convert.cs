using ConvertApiDotNet;
using ConvertApiDotNet.Model;

namespace bot.Helpers;

public static class Convert
{
    public static async Task ConvertTxtToPdf()
    {       
        ConvertApi convertApi = new ConvertApi("BdqUk2GXnWRXdP78");
        ConvertApiResponse result = await convertApi.ConvertAsync("txt", "pdf", new[]
        {
        new ConvertApiFileParam("text.txt")
        });

        
        var fileInfo = await result.SaveFileAsync("computer.pdf");
    }
}