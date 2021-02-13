﻿using System;
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

namespace RachelBot
{
    public class EventHandler
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _service;
        private readonly IStorageService _storage = new JsonStorageService();

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;

            (_service as IDisposable)?.Dispose();
            _service = new ServiceCollection()
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
