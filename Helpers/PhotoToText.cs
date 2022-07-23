using bot.Entity.Model;
using Newtonsoft.Json;

namespace bot.Helpers
{
    public static class PhotoToText
    {
        public static async Task<string?> GetText(string fileId)
        {
            string token = "2115821650:AAEQzeq_zd4YvVxfBmE-zVogQlSkiVUIb-0";
            string  urlFileId= $"https://api.telegram.org/bot{token}/getFile?file_id={fileId}";
            PhotoFile photoJson;
            using(HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(urlFileId);
                var content = await response.Content.ReadAsStringAsync();
                photoJson = JsonConvert.DeserializeObject<PhotoFile>(content);
             
            }


            string urlFilePath = $"https://api.telegram.org/file/bot{token}/{photoJson.result.file_path}";
            string qrCodeRead = $"http://api.qrserver.com/v1/read-qr-code/?fileurl={urlFilePath}";
            using(HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(qrCodeRead);
                var content = await response.Content.ReadAsStringAsync();
                var listQrCode = JsonConvert.DeserializeObject<List<QrCode>>(content);
                return listQrCode[0].Symbol[0].Data;
            }
        }
    }
}