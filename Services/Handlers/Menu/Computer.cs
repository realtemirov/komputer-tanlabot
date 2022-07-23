using System.Web;
using bot.Constants;
using bot.Services;
using bot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using bot.Entity;
using System.Text;
using Telegram.Bot.Types.InputFiles;

namespace bot.Services;

public partial class BotUpdateHandler
{
    private async Task ComputerAsync(ITelegramBotClient client, CallbackQuery query, CancellationToken token)
    {
        var message = query.Message;
        var from = message.From;

        await client.EditMessageCaptionAsync(
                            chatId: message.Chat.Id,
                            messageId: message.MessageId,
                            caption: _localizer["computer"],
                            replyMarkup: MarkupHelpers.GetInlineKeyboardMatrix(
                                StringConstants.ProgsResxToDictionary(_localizer["progs"].ToString().Split("|")), 3),
                            cancellationToken: token);
    }

    public async Task<Dictionary<string, string>> GetFromDbAsync(string query)
    {
        var progType = query switch
        {
            "office" => EProgType.Office,
            "graph" => EProgType.Graphic,
            "videomaker" => EProgType.VideoMaker,
            "ide" => EProgType.IDE,
            "browser" => EProgType.Browser,
            "social" => EProgType.SocialNetwork,
            "game" => EProgType.Game,
        };
        var dict = _progService.GetAllProgsAsync().Result.Where(p => p.ProgType == progType).ToDictionary(p => p.Query, p => p.Name);
        dict.Add("computer", "Nazad");
        return dict;
    }

    private async Task ReadyAsync(ITelegramBotClient client, CallbackQuery query, CancellationToken token)
    {
        var user = query.Message.Chat;
        var allChosenApp = _chosenAppService.GetAllChosenAppAsync().Result.Where(c => c.UserId == user.Id).Select( p=> p.ProgId ).ToList();
        
        if(allChosenApp.Count < 1  )
        {
            _logger.LogInformation("NULL");
            await AlertAsync(client, query, token, "Hali dasturlarni tanlamadingir");
            await client.EditMessageCaptionAsync(user.Id, query.Message.MessageId,
            caption: _localizer["ourservice"],
            replyMarkup: MarkupHelpers.GetInlineKeyboardMatrix(
                StringConstants.MenuResxToDictionary(_localizer["menu"].ToString().Split('|'))),
            cancellationToken: token);
        
            return;
        }
        _logger.LogInformation(" Chosen app NOT NULL");

        var progss = _progService.GetAllProgsAsync().Result.Where(p => allChosenApp.Contains(p.Id));
        
        var maxPoint = progss.Max(p => p.Point);
        
        _logger.LogInformation(" progss NOT NULL");

        var myComps = _computerService.GetAllCompsAsync().Result.Where(c => c.Grade >= maxPoint).ToList();

        var guid = Guid.NewGuid().ToString();
        await _computerService.DeleteMyComps(user.Id);
        foreach (var item in myComps)
        {
            var resultAddComp = _computerService.AddMyKompAsync( new MyComputer
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ComputerId = item.Id,
                Link = guid,
                CreatedAt = DateTimeOffset.Now

            }).Result;
        }
        await _chosenAppService.DeleteChosenAppAsync(user.Id);


        AlertAsync(client, query, token,"Ma'lumotlaringiz yuborildi. Tez orada yetib boradi");

        _logger.LogInformation("Convert File");
        await CompToText(myComps,user.Id);

        await HandleMenu(client, query.Message, token);
        using(var stream = new FileStream("computer.pdf",FileMode.Open))
        {
            await client.SendDocumentAsync(user.Id, new InputOnlineFile(stream,$"{user.FirstName}ning komputerlari.pdf"), cancellationToken: token);                
        }
        
        await client.DeleteMessageAsync(user.Id, query.Message.MessageId, token);
        
        _logger.LogInformation("File Send");
    }

    private async Task NoteSelectAsync(ITelegramBotClient client, CallbackQuery query, CancellationToken token)
    {
        
        var message = query.Message;
        var from = message.From;
        var prog = _progService.GetProgAsync(query.Data.ToString()).Result;
        if (prog is null)
        {
            _logger.LogError("Program is null");
        }
        

        _logger.LogInformation(query.Data);

        var res = _chosenAppService.AddChosenAppAsync(new ChosenApp
        {
            Id = Guid.NewGuid(),
            UserId = message.Chat.Id,
            ProgId = prog.Id,
            ChosenTime = DateTimeOffset.Now
        }).Result;
        
        
        var queryShort = query.Data.ToString().Split("-")[0];

        if (res.IsSuccess)
        {
            _logger.LogInformation(res.ErrorMessage);
            await client.EditMessageCaptionAsync(
                            chatId: message.Chat.Id,
                            messageId: message.MessageId,
                            caption: _progService.GetProgAsync(query.Data).Result.Name + " dasturi tanlandi",
                            replyMarkup:MarkupHelpers.GetInlineKeyboardMatrix(await GetFromDbAsync(queryShort),3),
                            cancellationToken: token);
        }
        else
        {
            
            AlertAsync(client, query, token,"Boshqa dasturni tanlang, bu avval tanlangan");
            
            _logger.LogInformation("alert chiqdi");
            _logger.LogInformation("alert boldi");                        
        }

    }

    public async Task CompToText(List<Kompyuter> mycomps,long userId)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var comp in mycomps)
        {
            sb.Append(
                $"Model: {comp.ModelName}\n" +
                $"Price: {comp.Price}\n" +
                $"Grade: {comp.Grade}\n" +
                $"GPU: {comp.GPU}\n" +
                $"Processor: {comp.Processor}\n" +
                $"RAM: {comp.RAM}\n" +
                $"Storage: {comp.Storage}\n" +
                $"ScreenSize: {comp.ScreenSize}\n" +
                $"OS: {comp.OS}\n" +
                $"Pic: {comp.LinkOfPic}\n" + 
                $"\n\n"
            );
        }
        
        System.IO.File.WriteAllText($"{userId}.txt",sb.ToString());
        
        await bot.Helpers.Convert.ConvertTxtToPdf(userId);
    }

    private async Task ProgsAsync(ITelegramBotClient client, CallbackQuery query, CancellationToken token)
    {
        var message = query.Message;
        var from = message.From;
        
        await client.EditMessageCaptionAsync(
                            chatId: message.Chat.Id,
                            messageId: message.MessageId,
                            caption: _localizer["office"],
                            replyMarkup:MarkupHelpers.GetInlineKeyboardMatrix(await GetFromDbAsync(query.Data.ToString()),3),
                            cancellationToken: token);
    }
}
