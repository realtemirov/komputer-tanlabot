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
}