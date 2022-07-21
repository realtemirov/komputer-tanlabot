using bot.Constants;
using bot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace bot.Services;

public partial class BotUpdateHandler
{
    private async Task HandleCallbackQueryAsync(ITelegramBotClient client, CallbackQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var from = query.From;

        _logger.LogInformation("Received CallbackQuery from {from.Firstname}: {query.Data}", from?.FirstName, query.Data);
        
        var handler = query.Data switch
        {
            "uz-Uz" or "ru-Ru" or "en-Us" => HandleLanguageAsync(client, query, token),
            "office" => OfficeAsync(client, query.Message, token),
            "office-msword" => OfficeWordAsync(client, query, token),
            "menu" => HandleMenu(client,query.Message,token),
            _ => Task.CompletedTask
        };

        await handler;
    }

    private async Task HandleMenu(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var from = message.From;
        await client.EditMessageCaptionAsync(
                            chatId: message.Chat.Id,
                            messageId: message.MessageId,
                            caption: "Kerakli dasturni tanlang: ",
                            replyMarkup: MarkupHelpers.GetInlineKeyboardMatrix(StringConstants.Programers, 3),
                            cancellationToken: token);
    }

}

