namespace MambaVPNbot;

public class Config(string botToken, string botPassword)
{
    public string BotToken { get; set; } = botToken;
    public string BotPassword { get; set; } = botPassword;
}