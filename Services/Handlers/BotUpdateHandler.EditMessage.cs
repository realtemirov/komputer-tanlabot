using Telegram.Bot;
using Telegram.Bot.Types;

namespace bot.Services;

public partial class BotUpdateHandler
{
    private async Task HandleEditMessageAsync(ITelegramBotClient client, Message? message, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(message);

        var from = message.From;

        _logger.LogInformation("Received Edit Message from {from.Firstname}: {message.Text}", from?.FirstName, message.Text);
    }
}