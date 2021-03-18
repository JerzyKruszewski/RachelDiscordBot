using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using RachelBot.Core.Configs;
using RachelBot.Services.Storage;
using RachelBot.Preconditions;
using RachelBot.Core.UserAccounts;
using RachelBot.Utils;
using RachelBot.Core.LevelingSystem;
using RachelBot.Lang;
using RachelBot.Core.RaffleSystem;
using System.Collections.Generic;

namespace RachelBot.Commands
{
    public class RaffleCommands : InteractiveBase<SocketCommandContext>
    {
        private readonly IStorageService _storage;

        public RaffleCommands(IStorageService storage)
        {
            _storage = storage;
        }

        [Command("Create Raffle")]
        [RequireStaff]
        public async Task CreateRaffle(bool canUsersJoin, [Remainder]string reward)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            Raffles raffles = new Raffles(guild.Id, _storage);

            raffles.CreateRaffle(reward, canUsersJoin);

            await Context.Channel.SendMessageAsync(alerts.GetAlert("SUCCESS"));
        }

        [Command("Add Tickets To User")]
        [RequireStaff]
        public async Task AddTickets(int id, SocketUser user, int amount)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            Raffles raffles = new Raffles(guild.Id, _storage);
            Raffle raffle = raffles.GetRaffle(id);

            raffles.AddTickets(raffle, user.Id, amount, byUser: false);

            await Context.Channel.SendMessageAsync(alerts.GetAlert("SUCCESS"));
        }

        [Command("Add Tickets To Role", RunMode = RunMode.Async)]
        [RequireStaff]
        public async Task AddTickets(int id, SocketRole role, int amount)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            Raffles raffles = new Raffles(guild.Id, _storage);
            Raffle raffle = raffles.GetRaffle(id);

            await guild.DownloadUsersAsync();

            foreach (SocketGuildUser user in guild.Users)
            {
                if (user.Roles.Contains(role))
                {
                    raffles.AddTickets(raffle, user.Id, amount, byUser: false);
                }
            }

            await Context.Channel.SendMessageAsync(alerts.GetAlert("SUCCESS"));
        }

        [Command("Roll Raffle")]
        [RequireStaff]
        public async Task RollRaffle(int id)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            Raffles raffles = new Raffles(guild.Id, _storage);

            Raffle raffle = raffles.Roll(raffles.GetRaffle(id));

            await Context.Channel.SendMessageAsync($"Winner of {raffle.Reward} is <@{raffle.Winner}>");
        }

        [Command("Join Raffle")]
        public async Task JoinRaffle(int id)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            Raffles raffles = new Raffles(guild.Id, _storage);

            if (raffles.AddTickets(raffles.GetRaffle(id), Context.User.Id, tickets: 1, byUser: true))
            {
                await Context.Channel.SendMessageAsync(alerts.GetAlert("SUCCESS"));
                return;
            }

            await Context.Channel.SendMessageAsync(alerts.GetAlert("FAILURE"));
        }

        [Command("Show Raffle")]
        public async Task ShowRaffle(int id)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            Raffles raffles = new Raffles(guild.Id, _storage);
            Raffle raffle = raffles.GetRaffle(id);

            string participants = "";

            foreach (KeyValuePair<ulong, int> participant in raffle.Tickets)
            {
                participants += $"\n<@{participant.Key}> - {participant.Value}";
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"Raffle #{raffle.Id} for {raffle.Reward}",
                Description = $"Raffle participants:{participants}",
                Color = new Color(1, 69, 44)
            };

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        [Command("Show Raffles")]
        public async Task ShowRaffles()
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            Raffles raffles = new Raffles(guild.Id, _storage);

            string message = "";

            foreach (Raffle raffle in raffles.GetRaffles())
            {
                if (raffle.IsEnded)
                {
                    message += $"~~{raffle.Id} - {raffle.Reward} ({raffle.Tickets.Count})~~\n";
                }
                else
                {
                    message += $"{raffle.Id} - {raffle.Reward} ({raffle.Tickets.Count})\n";
                }
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"Raffles",
                Description = message,
                Color = new Color(1, 69, 44)
            };

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
