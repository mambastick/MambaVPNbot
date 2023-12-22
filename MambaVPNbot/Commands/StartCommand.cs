using MambaVPNbot.Handlers;
using MambaVPNbot.VPN;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MambaVPNbot.Commands;

public class StartCommand
{
    public async Task SendWelcomeMessageAsync(ITelegramBotClient botClient, Update update,
        bool isCalledFromError = false)
    {
        try
        {
            var message = update.Message;
            var user = message.From;

            var content = isCalledFromError 
                ? $"{user.FirstName}, вы ввели <b>неверный пароль</b>, повторите попытку:"
                : $"Привет, {user.FirstName} !\n<b>Введите пароль</b>, чтобы продолжить:";
            
            await botClient.SendTextMessageAsync(user.Id, 
                content,
                parseMode: ParseMode.Html);
            new UserStateHandler().SetUserState(user.Id, UserState.EnterPassword);
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }
    
    public async Task GetPasswordAsync(ITelegramBotClient botClient, Update update)
    {
        var message = update.Message;
        var user = message.From;
        var enteredPassword = message.Text;
        
        try
        {
            if (enteredPassword != Bot.BotPassword)
                throw new Exception("Неверный пароль.");
            
            new UserStateHandler().SetUserState(user.Id, UserState.Unknown);
            await botClient.SendTextMessageAsync(
                user.Id,
                "Пароль <b>введён верно</b>.\n" +
                "Пожалуйста, подождите некоторое время, я сейчас <b>пришлю ваш профиль</b> для OpenVPN.",
                parseMode: ParseMode.Html
            );

            await new VpnProfile().SendUserProfileAsync(botClient, update);
        }
        catch (Exception ex) when (ex.Message == "Неверный пароль.")
        {
            await SendWelcomeMessageAsync(botClient, update, true);
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }
}