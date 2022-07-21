using System.Web;
using bot.Constants;
using bot.Services;
using bot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using bot.Entity;

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

        var myComp = _computerService.GetAllCompsAsync().Result.Where(c => c.Grade >= maxPoint).MinBy(c => c.Price);

        var resultAddComp = _computerService.AddMyKompAsync( new MyComputer
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ComputerId = myComp.Id,
            Link = "hello",
            CreatedAt = DateTimeOffset.Now

        }).Result;
        await _chosenAppService.DeleteChosenAppAsync(user.Id);
        string path = $@"Helpers/{myComp.Id}";
        var file = System.IO.File.Create(path + ".txt");
        file.Close();
        System.IO.File.WriteAllText(path + ".txt", myComp.Id.ToString());

        HandleMenu(client, query.Message, token);
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
            ProgsAsync(client,query,token);
        }
        else
        {
            AlertAsync(client, query, token);
            _logger.LogInformation(res.ErrorMessage);
        }

    }
}
