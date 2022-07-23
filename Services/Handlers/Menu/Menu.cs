using System.Web;
using bot.Constants;
using bot.Services;
using bot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using bot.Entity;
using Telegram.Bot.Types.InputFiles;

namespace bot.Services;

public partial class BotUpdateHandler
{
    private async Task HandleSetting(ITelegramBotClient client, CallbackQuery query, CancellationToken token)
    {
        var user = query.Message.Chat;
        await client.EditMessageCaptionAsync(user.Id, query.Message.MessageId,
            caption: _localizer["greeting",query.Message.Chat.FirstName],
            parseMode: ParseMode.Html,
            replyMarkup: MarkupHelpers.GetInlineKeyboardMatrix(StringConstants.LanguageNames,3),
            cancellationToken: token);
    }

    private async Task HandleMyComp(ITelegramBotClient client, CallbackQuery query, CancellationToken token)
    {
        var message = query.Message;
        var user = message.Chat;
        await AlertAsync(client, query, token, "Tez orada jo'natiladi");
        var mycomps = _computerService.GetAllMyCompsAsync().Result.Where(c => c.UserId == user.Id).Select(p => p.ComputerId).ToList();
        if(mycomps.Count < 1  )
        {
            _logger.LogInformation("NULL");
            AlertAsync(client, query, token, "Sizning komputerlaringiz hali yo'q");
            return;
        }
        _logger.LogInformation(" MyCOmps NOT NULL");

        var comps = _computerService.GetAllCompsAsync().Result.Where(c => mycomps.Contains(c.Id)).ToList();
        await CompToText(comps,user.Id);

        using(var stream = new FileStream("computer.pdf",FileMode.Open))
        {
            await client.SendDocumentAsync(user.Id, new InputOnlineFile(stream,$"{user.FirstName}ning komputerlari.pdf"), cancellationToken: token);                
        }
        _logger.LogInformation("File Send");

    }

    private async Task AlertAsync(ITelegramBotClient client, CallbackQuery query, CancellationToken token,string alert)
    {
        await client.AnswerCallbackQueryAsync(query.Id,alert,true, cancellationToken: token);
    }
    
    private async Task HandleMenu(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var root = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(root, "main.jpg");

        var bytes = await System.IO.File.ReadAllBytesAsync(filePath, token);
        using var stream = new MemoryStream(bytes);

        await client.SendPhotoAsync(
            message.Chat.Id,
            photo: stream,
            caption: _localizer["ourservice", message.From?.FirstName ?? "👻 Something went wrong"],
            replyMarkup: MarkupHelpers.GetInlineKeyboardMatrix(
                StringConstants.MenuResxToDictionary(_localizer["menu"].ToString().Split('|'))),
            cancellationToken: token);
        
    }
}