using System;
using System.Threading.Tasks;
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

namespace RachelBot
{
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

            var cmdConfig = new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            };

            (_commands as IDisposable)?.Dispose();
            _commands = new CommandService(cmdConfig);
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _service);

            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += HandleUserJoinAsync;
            _client.UserLeft += HandleUserLeftAsync;
        }

        private async Task HandleUserLeftAsync(SocketGuildUser arg)
        {
            SocketGuild guild = arg.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = string.Format(config.LeftMessage, arg.Mention, arg.Username, arg.Id, guild.Name),
                Color = new Color(255, 0, 0),
                ThumbnailUrl = arg.GetAvatarUrl()
            };

            ISocketMessageChannel channel = Utility.GetMessageChannelById(guild, config.OutChannelId);

            if (channel == null)
            {
                return;
            }

            await channel.SendMessageAsync("", embed: embed.Build());
        }

        private async Task HandleUserJoinAsync(SocketGuildUser arg)
        {
            SocketGuild guild = arg.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = string.Format(config.WelcomeMessage, arg.Mention, arg.Username, arg.Id, guild.Name),
                Color = new Color(0, 255, 0),
                ThumbnailUrl = arg.GetAvatarUrl()
            };

            ISocketMessageChannel channel = Utility.GetMessageChannelById(guild, config.InChannelId);

            if (channel == null)
            {
                return;
            }

            await channel.SendMessageAsync("", embed: embed.Build());
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

                GuildConfig config = new GuildConfigs(context.Guild.Id, _storage).GetGuildConfig();

                new LevelingHandler(_storage).UserSendMessage(context.User as SocketGuildUser);

                int argPos = 0;
                bool hasPrefix;
                if (config == null)
                {
                    hasPrefix = (msg.HasStringPrefix(ConfigurationManager.AppSettings["Prefix"], ref argPos)
                    || msg.HasMentionPrefix(_client.CurrentUser, ref argPos));
                }
                else
                {
                    hasPrefix = (msg.HasStringPrefix(config.GuildPrefix, ref argPos)
                    || msg.HasMentionPrefix(_client.CurrentUser, ref argPos));
                }

                if (hasPrefix)
                {
                    IResult result = await _commands.ExecuteAsync(context, argPos, _service);
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
