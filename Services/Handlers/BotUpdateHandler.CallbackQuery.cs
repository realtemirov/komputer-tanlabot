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
    }
}

