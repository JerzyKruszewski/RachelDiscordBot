using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;

namespace RachelBot
{
    public class EventHandler
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;

            (_service as IDisposable)?.Dispose();
            _service = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(new InteractiveService(_client))
                .BuildServiceProvider();

            var cmdConfig = new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            };

            (_commands as IDisposable)?.Dispose();
            _commands = new CommandService(cmdConfig);
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _service);

            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            try
            {
                if (arg.Channel is IPrivateChannel)
                {
                    return;
                }

                SocketUserMessage msg = arg as SocketUserMessage;
                if (msg == null) return;
                SocketCommandContext context = new SocketCommandContext(_client, msg);

                int argPos = 0;
                if (msg.HasStringPrefix(ConfigurationManager.AppSettings["Prefix"], ref argPos)
                    || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    var result = await _commands.ExecuteAsync(context, argPos, _service);
                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        Console.WriteLine(result.ErrorReason);
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
