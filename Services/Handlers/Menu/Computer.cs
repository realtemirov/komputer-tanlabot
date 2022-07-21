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
        
        var maxPoint = _progService.GetAllProgsAsync().Result.Where(p => allChosenApp.Contains(p.Id)).Max(p => p.Point);

        var myComps = _computerService.GetAllCompsAsync().Result.Where(c => c.Grade >= maxPoint).OrderBy(c => c.Price).ToList();

        var guid = Guid.NewGuid().ToString();

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


        AlertAsync(client, query, token,"Ma'lumotlaringiz yuborildi");

        await client.DeleteMessageAsync(user.Id, query.Message.MessageId, token);
        await CompToText(myComps);

        await HandleMenu(client, query.Message, token);
        using(var stream = new FileStream("computer.pdf",FileMode.Open))
        {
            await client.SendDocumentAsync(user.Id, new InputOnlineFile(stream,$"{user.FirstName}ning komputerlari.pdf"), cancellationToken: token);                
        }
    }

    private async Task ShowComputersAsync(ITelegramBotClient client, CallbackQuery query, CancellationToken token)
    {
        var message = query.Message;
        var from = message.From;
        var user = message.Chat;
        var allChosenApp = _chosenAppService.GetAllChosenAppAsync().Result.Where(c => c.UserId == user.Id).Select(p => p.ProgId).ToList();
        var maxPoint = _progService.GetAllProgsAsync().Result.Where(p => allChosenApp.Contains(p.Id)).Max(p => p.Point);
        var myComps = _computerService.GetAllCompsAsync().Result.Where(c => c.Grade >= maxPoint).OrderBy(c => c.Price).ToList();
        var comps = myComps.Select(c => $"{c.ModelName} {c.Price}").ToList();
        var compsStr = string.Join("\n", comps);
        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: compsStr,
                            replyMarkup: new InlineKeyboardMarkup(
                                new[]
                                {
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("Nazad", "menu")
                                    }
                                }),
                            cancellationToken: token);
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
        var queryShort = query.Data.ToString().Split("-")[0];


        _logger.LogInformation(query.Data);

        var res = _chosenAppService.AddChosenAppAsync(new ChosenApp
        {
            Id = Guid.NewGuid(),
            UserId = message.Chat.Id,
            ProgId = prog.Id,
            ChosenTime = DateTimeOffset.Now
        }).Result;
        
        

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

    public async Task CompToText(List<Kompyuter> mycomps)
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
        
        System.IO.File.WriteAllText("text.txt",sb.ToString());
        
        await bot.Helpers.Convert.ConvertTxtToPdf();
    }
}
