using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using RachelBot.Core.Configs;
using RachelBot.Services.Storage;
using RachelBot.Preconditions;
using RachelBot.Utils;
using RachelBot.Lang;
using RachelBot.Core.RaffleSystem;

namespace RachelBot.Commands;

[RequirePublicChannel]
public class RaffleCommands : InteractiveBase<SocketCommandContext>
{
    private readonly IStorageService _storage;

    public RaffleCommands(IStorageService storage)
    {
        _storage = storage;
    }

    [Command("Create Raffle")]
    [Alias("Stwórz Loterię", "Stworz Loterie")]
    [RequireStaff]
    public async Task CreateRaffle(bool canUsersJoin, [Remainder]string reward)
    {
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);
        Raffles raffles = new Raffles(guild.Id, _storage);

        raffles.CreateRaffle(reward, canUsersJoin);

        await Context.Channel.SendMessageAsync(alerts.GetAlert("RAFFLE_CREATED"));
    }

    [Command("Add Tickets To User")]
    [Alias("Dodaj Bilety Do Użytkownika", "Dodaj Bilety Do Uzytkownika")]
    [RequireStaff]
    public async Task AddTickets(int id, SocketUser user, int amount)
    {
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);
        Raffles raffles = new Raffles(guild.Id, _storage);
        Raffle raffle = raffles.GetRaffle(id);

        raffles.AddTickets(raffle, user.Id, amount, byUser: false);

        await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("ADDED_TICKET_TO_USER", amount, user.Username));
    }

    [Command("Add Tickets To Role", RunMode = RunMode.Async)]
    [Alias("Dodaj Bilety Do Roli")]
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

        await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("ADDED_TICKET_TO_ROLE", amount, role.Name));
    }

    [Command("Roll Raffle")]
    [Alias("Losuj")]
    [RequireStaff]
    public async Task RollRaffle(int id)
    {
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);
        Raffles raffles = new Raffles(guild.Id, _storage);

        Raffle raffle = raffles.Roll(raffles.GetRaffle(id));

        await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("CHECK_WINNER", raffle.Reward, raffle.Winner));
    }

    [Command("Join Raffle")]
    [Alias("Dołącz Do Loterii", "Dolacz Do Loterii")]
    public async Task JoinRaffle(int id)
    {
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);
        Raffles raffles = new Raffles(guild.Id, _storage);

        if (raffles.AddTickets(raffles.GetRaffle(id), Context.User.Id, tickets: 1, byUser: true))
        {
            await Context.Channel.SendMessageAsync(alerts.GetAlert("SUCCESSFULLY_JOINED_RAFFLE"));
            return;
        }

        await Context.Channel.SendMessageAsync(alerts.GetAlert("RAFFLE_JOIN_FAIL"));
    }

    [Command("Show Raffle")]
    [Alias("Loteria")]
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

            if (participants.Length > Utility.MessageLengthBuffer)
            {
                participants += "...";
                break;
            }
        }

        EmbedBuilder embed = new EmbedBuilder()
        {
            Title = alerts.GetFormattedAlert("RAFFLE_TITLE", raffle.Id, raffle.Reward),
            Description = alerts.GetFormattedAlert("RAFFLE", participants),
            Color = new Color(1, 69, 44)
        };

        await Context.Channel.SendMessageAsync(embed: embed.Build());
    }

    [Command("Show Raffles")]
    [Alias("Loterie")]
    public async Task ShowRaffles()
    {
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);
        Raffles raffles = new Raffles(guild.Id, _storage);

        string message = "";

        foreach (Raffle raffle in raffles.GetRaffles().OrderByDescending(r => r.Id))
        {
            if (raffle.IsEnded)
            {
                message += $"~~{raffle.Id} - {raffle.Reward} Winner: <@{raffle.Winner}>~~\n";
            }
            else
            {
                message += $"{raffle.Id} - {raffle.Reward} ({raffle.Tickets.Count})\n";
            }

            if (message.Length > Utility.MessageLengthBuffer)
            {
                message += "...";
                break;
            }
        }

        EmbedBuilder embed = new EmbedBuilder()
        {
            Title = alerts.GetAlert("RAFFLES_TITLE"),
            Description = message,
            Color = new Color(1, 69, 44)
        };

        await Context.Channel.SendMessageAsync(embed: embed.Build());
    }
}
