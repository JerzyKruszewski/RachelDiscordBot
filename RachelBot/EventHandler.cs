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
                if (arg is not SocketUserMessage msg) return;

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
                bool hasPrefix;
                if (config is null)
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
    }
}
