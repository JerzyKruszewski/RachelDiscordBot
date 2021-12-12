using System.Configuration;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using RachelBot.Services.Storage;
using RachelBot.Core.Configs;
using RachelBot.Utils;
using RachelBot.Core.LevelingSystem;

namespace RachelBot;

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
            .AddSingleton(new InteractiveService(_client))
            .BuildServiceProvider();

        CommandServiceConfig cmdConfig = new CommandServiceConfig
        {
            DefaultRunMode = RunMode.Async
        };

        (_commands as IDisposable)?.Dispose();
        _commands = new CommandService(cmdConfig);
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _service);

        _client.MessageReceived += HandleCommandAsync;
        _client.UserJoined += HandleUserJoinAsync;
        _client.UserLeft += HandleUserLeftAsync;
        _client.ButtonExecuted += HandleButtonClicked;
        _client.SelectMenuExecuted += HandleSelectMenuSelected;
    }

    private async Task HandleSelectMenuSelected(SocketMessageComponent arg)
    {
        try
        {
            SocketGuild guild = Utility.GetGuildFromSocketMessageComponent(_client, arg);

            switch (arg.Data.CustomId)
            {
                default:
                    //FollowUpAsync throwing an exception: Unknown webhook
                    await arg.Channel.SendMessageAsync("Ok");
                    break;
            }

            await arg.DeferAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}\n{ex.StackTrace}");
            Program.LogToFile($"ERROR: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private async Task HandleButtonClicked(SocketMessageComponent arg)
    {
        try
        {
            SocketGuild guild = Utility.GetGuildFromSocketMessageComponent(_client, arg);

            switch (arg.Data.CustomId)
            {
                case "IpsumId":
                    await arg.Channel.SendMessageAsync("NotOk");
                    break;
                default:
                    await arg.Channel.SendMessageAsync(guild.Name);
                    break;
            }

            await arg.DeferAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}\n{ex.StackTrace}");
            Program.LogToFile($"ERROR: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private async Task HandleUserLeftAsync(SocketGuildUser arg)
    {
        try
        {
            SocketGuild guild = arg.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();

            EmbedBuilder embed = new EmbedBuilder()
            {
                Description = string.Format(config.LeftMessage, arg.Mention, arg.Username, arg.Id, guild.Name),
                Color = new Color(255, 0, 0),
                ThumbnailUrl = arg.GetAvatarUrl()
            };

            ISocketMessageChannel channel = Utility.GetMessageChannelById(guild, config.OutChannelId);

            if (channel is null)
            {
                return;
            }

            await channel.SendMessageAsync("", embed: embed.Build());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}\n{ex.StackTrace}");
            Program.LogToFile($"ERROR: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private async Task HandleUserJoinAsync(SocketGuildUser arg)
    {
        try
        {
            SocketGuild guild = arg.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();

            EmbedBuilder embed = new EmbedBuilder()
            {
                Description = string.Format(config.WelcomeMessage, arg.Mention, arg.Username, arg.Id, guild.Name),
                Color = new Color(0, 255, 0),
                ThumbnailUrl = arg.GetAvatarUrl()
            };

            ISocketMessageChannel channel = Utility.GetMessageChannelById(guild, config.InChannelId);

            if (channel is null)
            {
                return;
            }

            await channel.SendMessageAsync("", embed: embed.Build());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}\n{ex.StackTrace}");
            Program.LogToFile($"ERROR: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
        try
        {
            if (arg is not SocketUserMessage msg)
            {
                return;
            }

            GuildConfig config = null;

            SocketCommandContext context = new SocketCommandContext(_client, msg);

            if (context.Channel is not IPrivateChannel)
            {
                config = new GuildConfigs(context.Guild.Id, _storage).GetGuildConfig();

                if (!config.ReactToBotMessages && context.User.IsBot)
                {
                    return;
                }

                try
                {
                    new LevelingHandler(_storage).UserSendMessage(context.User, context.Guild);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR with Leveling System: {ex}");
                    Program.LogToFile($"ERROR with Leveling System: {ex}");
                }
            }

            int argPos = 0;

            if (CheckIfMessageHasValidPrefix(msg, config, ref argPos))
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

    private bool CheckIfMessageHasValidPrefix(SocketUserMessage msg, GuildConfig config, ref int argPos)
    {
        if (config is null)
        {
            return (msg.HasStringPrefix(ConfigurationManager.AppSettings["Prefix"], ref argPos)
                    || msg.HasMentionPrefix(_client.CurrentUser, ref argPos));
        }

        return (msg.HasStringPrefix(config.GuildPrefix, ref argPos)
                || msg.HasMentionPrefix(_client.CurrentUser, ref argPos));
    }
}
