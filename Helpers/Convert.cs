using ConvertApiDotNet;
using ConvertApiDotNet.Model;

namespace bot.Helpers;

public class Convert
{
    public async Task ConvertTxtToPdf(string path)
    {       
        ConvertApi convertApi = new ConvertApi("BdqUk2GXnWRXdP78");
        ConvertApiResponse result = await convertApi.ConvertAsync("txt", "pdf", new[]
        {
        new ConvertApiFileParam(path + ".txt")
        });

        
        var fileInfo = await result.SaveFileAsync(path + ".pdf");
    }
}