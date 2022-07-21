using System.Web;
using bot.Constants;
using bot.Entity;
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
        
        var reesss = await _userService.UpdateLanguageCodeAsync(message?.Chat?.Id, cultureString);
        
        await client.DeleteMessageAsync(message?.Chat?.Id, message.MessageId, token);

        await client.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto, token);
        await HandleMenu(client, query, token);
    }

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