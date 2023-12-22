using System.Text.Json;
using MLoggerService;

namespace MambaVPNbot;

class Program
{
    private static MLogger Logger { get; set; }
    private static async Task Main()
    {
        Logger = new MLogger();
        var config = LoadConfigFromFile();
        
        if (config is not null)
        {
            Logger.LogSuccess("Config loaded successfully.");
        }
        else
        {
            config = InitializeConfig();
        }

        var bot = new Bot(config, Logger);
        await bot.StartAsync();
        await Task.Delay(-1);
    }

    private static Config? LoadConfigFromFile()
    {
        if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"))) 
            return null;
        
        var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"));
        
        try
        { 
            return JsonSerializer.Deserialize<Config>(json);
        }
        catch (JsonException ex)
        {
            Logger.LogError($"Error deserializing config: {ex.Message}");
            return null;
        }
    }

    private static Config InitializeConfig()
    {
        Logger.LogInformation("Enter the bot token:");
        var token = Console.ReadLine();

        Logger.LogInformation("Enter the bot password: ");
        var botPassword = Console.ReadLine();

        Logger.LogInformation("Enter generate certificate script path: ");
        var certificatePath = Console.ReadLine();
        
        Logger.LogInformation("Enter the script password: ");
        var scriptPassword = Console.ReadLine();

        var config = new Config(token, botPassword, certificatePath, scriptPassword);
        
        SaveConfigToFile(config);

        return config;
    }

    private static void SaveConfigToFile(Config config)
    {
        try
        {
            var jsonConfig = JsonSerializer.Serialize(config);
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"), jsonConfig);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error saving config: {ex.Message}");
        }
    }
}