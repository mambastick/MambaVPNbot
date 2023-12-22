using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace MambaVPNbot.VPN;

public class VpnProfile
{
    public async Task SendUserProfileAsync(ITelegramBotClient botClient, Update update)
    {
        var message = update.Message;
        var user = message.From;

        var userProfile = GenerateCertificate(user);
        await using Stream stream = File.OpenRead(userProfile);
        await botClient.SendDocumentAsync(
            chatId: user.Id,
            document: InputFile.FromStream(stream: stream, fileName: $"{user.Id}.ovpn"),
            caption: "Вот ваш OpenVPN профиль.");
    }

    private string GenerateCertificate(User user)
    {
        var scriptPath = "/etc/openvpn/gen-sert.sh";
        if (!File.Exists(scriptPath))
        {
            Bot.Logger.LogFatal("Bash script not found!");
            return string.Empty;
        }
        
        var processInfo = new ProcessStartInfo();
        processInfo.FileName = "/bin/bash";
        processInfo.RedirectStandardInput = true;
        processInfo.RedirectStandardOutput = true;
        processInfo.UseShellExecute = false;
        
        processInfo.Arguments = $"{scriptPath} {user.Id}";


        Process process = new Process();
        process.StartInfo = processInfo;
        process.Start();
        string requiredData = ""; // password
        
        using (StreamWriter sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                sw.WriteLine(requiredData); 
            }
        }
        
        using (StreamReader sr = process.StandardOutput)
        {
            string output = sr.ReadToEnd();
            Bot.Logger.LogInformation("Script output: " + output);
        }

        process.WaitForExit();
        process.Close();
        
        return $"/tmp/keys/{user.Id}.ovpn";
    }
}