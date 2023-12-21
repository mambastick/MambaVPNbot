using MambaVPNbot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MambaVPNbot.Handlers;

public class UserStateHandler
{
    private static Dictionary<long, UserState> UserStates = new();

    private UserState GetUserState(long userId)
    {
        UserStates.TryGetValue(userId, out var state);
        return state;
    }

    public void SetUserState(long userId, UserState userState) => UserStates[userId] = userState;

    public async Task UserStateHandlerAsync(ITelegramBotClient botClient, Update update)
    {
        try
        {
            var message = update.Message;
            if (GetUserState(message.From.Id) is UserState.EnterPassword)
            {
                await new StartCommand().GetPasswordAsync(botClient, update);
                return;
            }
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.Message);
        }
        
    }
}

public enum UserState
{
    EnterPassword,
    Unknown
}