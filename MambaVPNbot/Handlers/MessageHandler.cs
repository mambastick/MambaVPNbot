using MambaVPNbot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MambaVPNbot.Handlers;

public class MessageHandler
{
    public async Task MessageHandlerAsync(ITelegramBotClient botClient, Update update)
    {
        try
        {
            var message = update.Message;
            switch (message.Type)
            {
                case MessageType.Text:
                    var text = message.Text;
                    if (IsBotCommand(text))
                    {
                        await new CommandsHandler().CommandsHandlerAsync(botClient, update);
                        return;
                    }

                    await new UserStateHandler().UserStateHandlerAsync(botClient, update);
                    
                    return;
            }
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.Message);
        }
    }
    
    private static bool IsBotCommand(string text) => !string.IsNullOrEmpty(text) && text.StartsWith("/");
}