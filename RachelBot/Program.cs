using System.Configuration;
using System.IO;
using System.Text;
using Discord;
using Discord.WebSocket;

namespace RachelBot;

internal class Program
{
    private const string LogFilePath = "./Log.txt";

    private DiscordSocketClient _client;
    private EventHandler _handler;

    private static void Main() => new Program().RunBotAsync().GetAwaiter().GetResult();

    private async Task RunBotAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["Token"]))
            {
                return;
            }

            _client?.Dispose();
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 0, 
                AlwaysDownloadUsers = true
            });

            await InitializationClient();
            await InitializationLogs();

            await Task.Delay(-1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            LogToFile($"ERROR: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private async Task InitializationClient()
    {
        await _client.SetGameAsync(ConfigurationManager.AppSettings["Game"]);
        await LoginAsync();
        await HandlerInitialize();
    }

    private async Task LoginAsync()
    {
        await _client.LoginAsync(TokenType.Bot, ConfigurationManager.AppSettings["Token"]);
        await _client.StartAsync();
    }

    private async Task HandlerInitialize()
    {
        _handler = new EventHandler();
        await _handler.InitializeAsync(_client);
    }

    private Task InitializationLogs()
    {
        _client.Log += BotLog;

        return Task.CompletedTask;
    }

    private Task BotLog(LogMessage msg)
    {
        Console.WriteLine(msg.Message);

        LogToFile(msg.Message);

        return Task.CompletedTask;
    }

    public static void LogToFile(string message)
    {
        using StreamWriter writer = new StreamWriter(LogFilePath, true, Encoding.UTF8);

        writer.WriteLine(message);
    }
}
