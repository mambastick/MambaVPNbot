using MambaVPNbot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MambaVPNbot.Handlers;

public class CommandsHandler
{
    public async Task CommandsHandlerAsync(ITelegramBotClient botClient, Update update)
    {
        try
        {
            var message = update.Message;
            var command = message.Text;
            var user = message.From;
            
            Bot.Logger.LogInformation(
                $"{user.FirstName} {user.LastName} | {user.Username} ({user.Id}) used command {command}");

            switch (command)
            {
                case "/start":
                    await new StartCommand().SendWelcomeMessageAsync(botClient, update);
                    return;
            }
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }
}