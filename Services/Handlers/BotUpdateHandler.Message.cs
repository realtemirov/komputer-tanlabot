using System.Web;
using bot.Constants;
using bot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace bot.Services;

public partial class BotUpdateHandler
{
    private async Task HandleMessageAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(message);

        var from = message.From;
        _logger.LogInformation("Received message from {from.Firstname}", from?.FirstName);

        var handler = message.Type switch
        {
            MessageType.Text => HandleTextMessageAsync(client, message, token),
            _ => HandleUnknownMessageAsync(client, message, token)
        };
        
        await handler;
    }

    private Task HandleUnknownMessageAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {
        _logger.LogInformation("Received message type {message.Type}", message.Type);

        return Task.CompletedTask;
    }

    private async Task HandleTextMessageAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var from = message.From;
        _logger.LogInformation("From: {from.Firstname}", from?.FirstName);

        var handler = message.Text switch
        {
            "/start" => HandleStartAsync(client, message, token),
            _ => Task.CompletedTask
        };
        
        await handler;
        
    }

    private async Task HandleLanguageAsync(ITelegramBotClient client, CallbackQuery query, CancellationToken token)
    {
        var message = query.Message;
        var cultureString = StringConstants.LanguageNames.FirstOrDefault(v => v.Key == query.Data).Key;
        _logger.LogInformation("user {message.From.Id}", message?.From?.Id);
        var reesss = await _userService.UpdateLanguageCodeAsync(message?.From?.Id, cultureString);
        _logger.LogInformation("set lang {reesss.ErrorMessage} {ciultureString}",reesss.ErrorMessage,cultureString);
        await client.DeleteMessageAsync(message?.Chat?.Id, message.MessageId, token);

        await client.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto, token);

        var root = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(root, "main.jpg");

        var bytes = await System.IO.File.ReadAllBytesAsync(filePath, token);

        using var stream = new MemoryStream(bytes);

        await client.SendPhotoAsync(
            message.Chat.Id,
            photo: stream,
            caption: _localizer["ourservice", message.From?.FirstName ?? "👻 Something went wrong"],
            replyMarkup: MarkupHelpers.GetInlineKeyboardMatrix(
                StringConstants.Menu),
            cancellationToken: token);
    }

    // start bosilganda
    private async Task HandleStartAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {

        var from = message.From;
        
        var root = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(root, "main.jpg");
        var bytes = await System.IO.File.ReadAllBytesAsync(filePath, token);
        using var stream = new MemoryStream(bytes);
        await client.DeleteMessageAsync(message.Chat.Id, message.MessageId, token);
        await client.SendPhotoAsync(
                            chatId: message.Chat.Id,
                            photo: stream,
                            caption: _localizer["greeting", from?.FirstName ?? "👻"],
                            replyMarkup: MarkupHelpers.GetInlineKeyboardMatrix(StringConstants.LanguageNames,3),
                            parseMode: ParseMode.Html,
                            cancellationToken: token);
    }
}