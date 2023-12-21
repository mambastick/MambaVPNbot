using System.Text.Json;
using MLoggerService;

namespace MambaVPNbot;

class Program
{
    private static MLogger Logger { get; set; }
    private static Config BotConfig { get; set; }
    
    private static async Task Main()
    {
        Logger = new MLogger();
        
        if (LoadConfigFromFile())
        {
            Logger.LogSuccess("Config loaded successfully.");
        }
        else
        {
            InitializeConfig();
        }

        var bot = new Bot(BotConfig.BotToken, Logger, BotConfig.BotPassword);
        await bot.StartAsync();
        await Task.Delay(-1);
    }

    private static bool LoadConfigFromFile()
    {
        if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"))) return false;
        var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"));
        try
        {
            BotConfig = JsonSerializer.Deserialize<Config>(json);
            return true;
        }
        catch (JsonException ex)
        {
            Logger.LogError($"Error deserializing config: {ex.Message}");
            return false;
        }
    }

    private static void InitializeConfig()
    {
        Logger.LogInformation("Enter the bot token:");
        var token = Console.ReadLine();

        Logger.LogInformation("Enter the bot password: ");
        var botPassword = Console.ReadLine();

        BotConfig = new Config(token, botPassword);

        SaveConfigToFile();
    }

    private static void SaveConfigToFile()
    {
        try
        {
            var jsonConfig = JsonSerializer.Serialize(BotConfig);
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"), jsonConfig);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error saving config: {ex.Message}");
        }
    }
}