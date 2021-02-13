using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace RachelBot
{
    internal class Program
    {
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
            return Task.CompletedTask;
        }
    }
}
