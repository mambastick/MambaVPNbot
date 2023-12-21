using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace MambaVPNbot.Handlers.General;

public class ErrorHandler
{
    public Task GetApiError(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Bot.Logger?.LogError(errorMessage);

        return Task.CompletedTask;
    }
}