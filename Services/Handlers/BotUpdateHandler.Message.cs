using System.Globalization;
using System.Web;
using bot.Constants;
using bot.Entity;
using bot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
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
            MessageType.Photo => HandlePhotoMessageAsync(client, message, token),
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
            _ => TextToPicAsync(client, message, token)
        };
        
        await handler;
    }

    private async Task HandlePhotoMessageAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {
        await client.SendTextMessageAsync(message.Chat.Id, "<strong> Biroz kuting ma'lumotlar ishlanmoqda.. </strong>",parseMode:ParseMode.Html, replyToMessageId: message.MessageId, cancellationToken: token);

        _logger.LogInformation("Photo message received: {message.Photo.Length}",message.Photo.Length);
        var result = PhotoToText.GetText(message.Photo[0].FileId).Result;
        string data ="" ;
        if (result is not null)
        {   
            data = $"<strong>Text:</strong> <code>{result}</code> " +
        $"\n<strong>Date:</strong> <code>{DateTime.Now}</code>";
        }
        else data = result;
            
        
        await client.SendTextMessageAsync(message.Chat.Id,data ?? "Bu rasmda <strong>qrcode</strong> mavjud emas",parseMode:ParseMode.Html,cancellationToken: token);
    }

    private async Task HandleLanguageAsync(ITelegramBotClient client, CallbackQuery query, CancellationToken token)
    {
        var message = query.Message;
        
        var cultureString = StringConstants.LanguageNames.FirstOrDefault(v => v.Key == query.Data).Key;
        
        var reesss = await _userService.UpdateLanguageCodeAsync(message?.Chat?.Id, cultureString);
        
        CultureInfo.CurrentCulture = new CultureInfo(cultureString);
        CultureInfo.CurrentUICulture = new CultureInfo(cultureString);
        
        await client.DeleteMessageAsync(message.Chat.Id, message.MessageId, token);
        await client.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto, token);
        
        await HandleMenu(client, message, token);
    }

    private async Task TextToPicAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {
        _logger.LogInformation("From: {from.Firstname}", message.Text);

        string baseUrl = "https://api.qrserver.com/v1/create-qr-code/?size=250x250&data=";

        using (HttpClient httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync(baseUrl + message.Text);
            var content = await response.Content.ReadAsByteArrayAsync();
            using var stream = new MemoryStream(content);
            await client.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: new InputOnlineFile(stream, $"{message.Text}.png"),
                caption: $"<strong>Text</strong>:  <code>{message.Text}</code>",
                parseMode: ParseMode.Html,
                replyToMessageId: message.MessageId,
                cancellationToken: token);
        }        
    }


    private async Task HandleStartAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {

        var from = message.From;
        var user = _userService.Exists(from.Id).Result;
        await client.DeleteMessageAsync(message.Chat.Id, message.MessageId, token);
        _logger.LogInformation(user.ToString());
        _logger.LogInformation("Tilni tanladi");
        if(!user)
        {
            var root = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(root, "main.jpg");
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath, token);
            using var stream = new MemoryStream(bytes);
            await client.SendPhotoAsync(
                            chatId: message.Chat.Id,
                            photo: stream,
                            caption: _localizer["greeting", from?.FirstName ?? "👻"],
                            replyMarkup: MarkupHelpers.GetInlineKeyboardMatrix(StringConstants.LanguageNames,3),
                            parseMode: ParseMode.Html,
                            cancellationToken: token);
            await CreateUser(client,message,token);
        }
        else
        {
            HandleMenu(client, message, token);
        }
        
    }
    private async Task CreateUser(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var from = message.From;
        var result = await _userService.AddUserAsync(new Entity.User()
        {
            FirstName = from.FirstName,
            LastName = from.LastName,
            // ChatId = ( update.Message == null ? update.CallbackQuery.Message.Chat.Id:update.Message.Chat.Id),
            IsBot = from.IsBot,
            UserId = from.Id,
            Username = from.Username,
            LanguageCode = from.LanguageCode,
            CreatedAt = DateTimeOffset.UtcNow,
            LastInteractionAt = DateTimeOffset.UtcNow
        });


        if(result.IsSuccess)
        {
            _logger.LogInformation("New user added: {from.Id}");
        }
        else
        {
            _logger.LogInformation("User not added: {from.Id}, {result.ErrorMessage}");
        }
    }
}