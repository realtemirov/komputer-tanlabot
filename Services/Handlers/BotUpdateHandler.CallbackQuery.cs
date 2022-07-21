using System.Globalization;
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
        var querySplit = query.Data.Split('-');
        var handler = querySplit[0] switch
        {
            "uz" or "ru" or "en" => HandleLanguageAsync(client, query, token),
            "computer" => ComputerAsync(client, query, token),
            "menu" => HandleMenu(client,query,token),
            _ => Task.CompletedTask
        };

        await handler;
    }
}

