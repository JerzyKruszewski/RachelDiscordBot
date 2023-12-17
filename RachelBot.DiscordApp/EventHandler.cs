using System.Configuration;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RachelBot.Services.Storage;

namespace RachelBot.DiscordApp;

public class EventHandler
{
    private DiscordSocketClient _client;
    private CommandService _commands;
    private IServiceProvider _service;
    private readonly IStorageService _storage = new JsonStorageService();
    private readonly Random _random = new Random();

    public async Task InitializeAsync(DiscordSocketClient client)
    {
        _client = client;

        (_service as IDisposable)?.Dispose();
        _service = new ServiceCollection()
            .AddSingleton(_random)
            .AddSingleton(_storage)
            .AddSingleton(_client)
            .BuildServiceProvider();

        CommandServiceConfig cmdConfig = new CommandServiceConfig
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
            if (arg is not SocketUserMessage msg)
            {
                return;
            }

            SocketCommandContext context = new SocketCommandContext(_client, msg);

            int argPos = 0;

            if (CheckIfMessageHasValidPrefix(msg, ref argPos))
            {
                IResult result = await _commands.ExecuteAsync(context, argPos, _service);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.WriteLine(result.ToString());
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                    Program.LogToFile($"WARNING: {result.ErrorReason}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex}");
            Program.LogToFile($"ERROR: {ex}");
        }
    }

    private bool CheckIfMessageHasValidPrefix(SocketUserMessage msg, ref int argPos)
    {
        return (msg.HasStringPrefix(ConfigurationManager.AppSettings["Prefix"], ref argPos)
                    || msg.HasMentionPrefix(_client.CurrentUser, ref argPos));
    }
}
