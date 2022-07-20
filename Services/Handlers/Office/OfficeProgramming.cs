using System.Web;
using bot.Constants;
using bot.Services;
using bot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace bot.Services;

public partial class BotUpdateHandler
{
    // start bosilganda
    private async Task OfficeAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var from = message.From;
        
        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "office bosildi",
                            cancellationToken: token);
    }
}