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

    




    private async Task AlertAsync(ITelegramBotClient client, CallbackQuery query, CancellationToken token)
    {
        var message = query.Message;
        var from = message.From;

        await client.AnswerCallbackQueryAsync(query.Id,"Bu dasturni tanladingiz",true, cancellationToken: token);
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