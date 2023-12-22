using MambaVPNbot.Handlers;
using MambaVPNbot.Handlers.General;
using MLoggerService;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace MambaVPNbot;

public class Bot
{
    public Bot(Config config, MLogger logger)
    {
        Logger = logger;
        Logger.LogProcess("Initializing bot...");

        BotClient = new TelegramBotClient(config.BotToken);
        Config = config;
        
        Logger.LogSuccess("Bot has been initialized.");
    }

    public static Config Config { get; set; }
    public static MLogger Logger { get; private set; }
    private ITelegramBotClient BotClient { get; }
    
    public Task StartAsync()
    {
        Logger.LogProcess("Start receive updates...");
        
        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions()
        {
            ThrowPendingUpdates = true,
            AllowedUpdates =
            [
                UpdateType.Message,
                UpdateType.CallbackQuery,
            ]
        };
        
        _ = BotClient.ReceiveAsync(
            new UpdateHandler().GetUpdatesAsync,
            new ErrorHandler().GetApiError,
            receiverOptions, 
            cancellationToken: cts.Token);
        
        Logger.LogSuccess("Bot receiving updates.");
        return Task.CompletedTask;
    }
}