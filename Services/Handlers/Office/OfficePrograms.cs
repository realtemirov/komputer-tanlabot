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
    
    private async Task OfficeAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var from = message.From;
        
        await client.EditMessageCaptionAsync(
                            chatId: message.Chat.Id,
                            messageId: message.MessageId,
                            caption: "📁 Office dasturlardan ishlatadiganlaringizni tanlang: ",
                            replyMarkup:MarkupHelpers.GetInlineKeyboardMatrix(StringConstants.OfficePrograms,3),
                            cancellationToken: token);


    }

    private async Task OfficeWordAsync(ITelegramBotClient client, CallbackQuery message, CancellationToken token)
    {
        var from = message.From;

        /*var result1 = await _chosenAppService.AddChosenAppAsync(new ChosenApp()
        {
            Id = Guid.NewGuid(),
            UserId = from.Id,
            ProgId = await _progService.GetProgAsync(message.Data).
            ChosenTime = DateTimeOffset.Now,
        });*/
        _logger.LogInformation("Hello");
    }
    
}