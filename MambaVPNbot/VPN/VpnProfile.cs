using System.Diagnostics;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace MambaVPNbot.VPN;

public class VpnProfile
{
    public void GetUserVpnProfile(User user)
    {
        
    }

    private void RunGenerateSertificatesScript(User user)
    {
        var scriptPath = "/etc/openvpn/gen-sert.sh";
        
        if (!File.Exists(scriptPath))
        {
            Bot.Logger.LogFatal("Bash script not found!");
            return;
        }
        
        var arguments = $"{user.FirstName}-{user.Id}";
        
        using var process = new Process();
        process.StartInfo.FileName = "bash";
        process.StartInfo.Arguments = $"-c \"{scriptPath} {arguments}\"";
        
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        
        process.Start();
        
        var output = process.StandardOutput.ReadToEnd();
        
        process.WaitForExit();
        
        Bot.Logger.LogInformation($"Script output: {output}");
    }
}