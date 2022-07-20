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
    private async Task HandleMessageAsync(ITelegramBotClient client, Message? message, CancellationToken token)
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
            "O'zbekcha" or "Русский" or "English" => HandleLanguageAsync(client, message, token),
            _ => Task.CompletedTask
        };

        await handler;
        
    }

    private async Task HandleLanguageAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var cultureString = StringConstants.LanguageNames.FirstOrDefault(v => v.Value == message.Text).Key;
        await _userService.UpdateLanguageCodeAsync(message.From.Id, cultureString);

        await client.DeleteMessageAsync(message.Chat.Id, message.MessageId, token);

        await client.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto, token);

        var photoUrl = "https://ilmhub.uz/assets/images/courses/netbootcamp.png";

        var root = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(root, "backenddotnet.png");

        var bytes = await System.IO.File.ReadAllBytesAsync(filePath, token);

        using var stream = new MemoryStream(bytes);

        await client.SendPhotoAsync(
            message.Chat.Id,
            photo: stream,
            caption: "Ilmhub IT School",
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: token);
    }

    private async Task HandleStartAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var from = message.From;

        await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _localizer["greeting", from?.FirstName ?? "👻"],
                replyMarkup: MarkupHelpers.GetReplyKeyboardMarkup(StringConstants.LanguageNames.Values.ToArray(), 3),
                parseMode: ParseMode.Html,
                cancellationToken: token);
    }
}