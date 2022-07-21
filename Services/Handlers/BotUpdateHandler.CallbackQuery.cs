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
        
        var handler = query.Data switch
        {
            "uz-Uz" or "ru-Ru" or "en-Us" => HandleLanguageAsync(client, query, token),
            "computer" => ComputerAsync(client, query, token),
            "office" or "graph" or "videomaker" or "ide" or "browser" or "social" or "game" => ProgsAsync(client, query, token),
            "menu" => HandleMenu(client,query.Message,token),
            "ready" => ReadyAsync(client, query, token),
            _ => NoteSelectAsync(client, query, token)
        };

        await handler;
    }
}

