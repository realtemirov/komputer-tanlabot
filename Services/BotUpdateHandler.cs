using System.Globalization;
using bot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace bot.Services;

public partial class BotUpdateHandler : IUpdateHandler
{
    private readonly ILogger<BotUpdateHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IStringLocalizer _localizer;
    private UserService _userService;
    private ChosenAppService _chosenAppService;
    private ProgService _progService;

    private ComputerService _computerService;
    

    public BotUpdateHandler(
        ILogger<BotUpdateHandler> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }
    

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Error occured with Telegram Bot: {e.Message}", exception);
    
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        _computerService = scope.ServiceProvider.GetRequiredService<ComputerService>();
        _userService = scope.ServiceProvider.GetRequiredService<UserService>();
        _chosenAppService = scope.ServiceProvider.GetRequiredService<ChosenAppService>();
        _progService = scope.ServiceProvider.GetRequiredService<ProgService>();
        

        var culture = await GetCultureForUser(update);
        // var culture = new CultureInfo("uz-Uz");

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        _localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<BotLocalizer>>();

        var handler = update.Type switch
        {
            UpdateType.Message => HandleMessageAsync(botClient, update.Message, cancellationToken),
            UpdateType.EditedMessage => HandleEditMessageAsync(botClient, update.EditedMessage, cancellationToken),
            UpdateType.CallbackQuery => HandleCallbackQueryAsync(botClient, update.CallbackQuery, cancellationToken),
            // handle other updates
            _ => HandleUnknownUpdate(botClient, update, cancellationToken)
        };

        try
        {
            await handler;
        }
        catch(Exception e)
        {
            await HandlePollingErrorAsync(botClient, e, cancellationToken);
        }
    }

    private async Task<CultureInfo> GetCultureForUser(Update update)
    {
        User from = update.Type switch
        {
            UpdateType.Message => update?.Message?.From,
            UpdateType.EditedMessage => update?.EditedMessage?.From,
            UpdateType.CallbackQuery => update?.CallbackQuery?.From,
            UpdateType.InlineQuery => update?.InlineQuery?.From,
            _ => update?.Message?.From
        };


        

        var language = await _userService.GetLanguageCodeAsync(from.Id);
        _logger.LogInformation($"Language set to: {language}");
        
        return new CultureInfo(language ?? "uz-Uz");
    }

    private Task HandleUnknownUpdate(ITelegramBotClient client, Update update, CancellationToken token)
    {
        _logger.LogInformation("Update type {update.Type} received", update.Type);

        return Task.CompletedTask;
    }
}