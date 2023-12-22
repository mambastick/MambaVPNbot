namespace MambaVPNbot;

public class Config(string botToken, string botPassword, string certScriptPath, string passwordCaCert)
{
    public string BotToken { get; init; } = botToken;
    public string BotPassword { get; init; } = botPassword;

    public string CertScriptPath { get; init; } = certScriptPath;
    public string PasswordCaCert { get; init; } = passwordCaCert;
}