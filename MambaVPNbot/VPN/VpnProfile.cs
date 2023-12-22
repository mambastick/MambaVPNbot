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

        var userProfile = GenerateSertificate(user);
        await using Stream stream = File.OpenRead(userProfile);
        await botClient.SendDocumentAsync(
            chatId: user.Id,
            document: InputFile.FromStream(stream: stream, fileName: $"{user.Id}.ovpn"),
            caption: "Вот ваш OpenVPN профиль.");
    }

    private string GenerateSertificate(User user)
    {
        var scriptPath = "/etc/openvpn/gen-sert.sh";
        if (!File.Exists(scriptPath))
        {
            Bot.Logger.LogFatal("Bash script not found!");
            return string.Empty;
        }

        // Создание нового процесса для выполнения sh скрипта
        ProcessStartInfo processInfo = new ProcessStartInfo();
        processInfo.FileName = "/bin/bash"; // Указываем оболочку, которая выполнит скрипт
        processInfo.RedirectStandardInput = true;
        processInfo.RedirectStandardOutput = true;
        processInfo.UseShellExecute = false;

        // Указываем путь к sh скрипту
        processInfo.Arguments = $"{scriptPath} {user.Id}";

        // Создаем новый процесс
        Process process = new Process();
        process.StartInfo = processInfo;
        process.Start();

        // Ожидание запроса данных от скрипта
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