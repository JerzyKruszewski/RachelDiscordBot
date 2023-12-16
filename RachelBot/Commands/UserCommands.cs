using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Rest;
using Discord.Addons.Interactive;
using RachelBot.Core.Configs;
using RachelBot.Services.Storage;
using RachelBot.Core.UserAccounts;
using RachelBot.Utils;
using RachelBot.Preconditions;
using RachelBot.Core.LevelingSystem;
using RachelBot.Lang;
using RachelBot.Core.Dialogues;

namespace RachelBot.Commands;

[RequirePublicChannel]
public class UserCommands : InteractiveBase<SocketCommandContext>
{
    private readonly IStorageService _storage;

    public UserCommands(IStorageService storage)
    {
        _storage = storage;
    }

    [Command("Status")]
    public async Task CheckStatus(SocketGuildUser user = null)
    {
        if (user is null)
        {
            user = Context.User as SocketGuildUser;
        }
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);
        UserAccounts accounts = new UserAccounts(guild.Id, _storage);
        UserAccount account = accounts.GetUserAccount(user.Id);

        accounts.DeleteExpiredWarnings(account);

        string message = Utility.GetStatusMessage(user, config, alerts, account);

        await Context.Channel.SendMessageAsync(message);
    }

    [Command("Warns")]
    [Alias("Ostrzeżenia", "Ostrzezenia")]
    public async Task CheckWarns(SocketGuildUser user = null)
    {
        if (user is null)
        {
            user = Context.User as SocketGuildUser;
        }
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);
        UserAccounts accounts = new UserAccounts(guild.Id, _storage);
        UserAccount account = accounts.GetUserAccount(user.Id);

        accounts.DeleteExpiredWarnings(account);
        string message = GetUserWarningsMessage(user, config, alerts, account);

        await Context.Channel.SendMessageAsync(message);
    }

    [Command("Praises")]
    [Alias("Pochwały", "Pochwaly")]
    public async Task CheckPraises(SocketGuildUser user = null)
    {
        if (user is null)
        {
            user = Context.User as SocketGuildUser;
        }
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);
        UserAccounts accounts = new UserAccounts(guild.Id, _storage);
        UserAccount account = accounts.GetUserAccount(user.Id);

        StringBuilder praiseList = new StringBuilder();

        foreach (Praise praise in account.Praises)
        {
            praiseList.Append(alerts.GetFormattedAlert("PARSE_PRAISE", praise.Id, praise.Reason, praise.GivenAt.ToString(@"dd\/MM\/yyyy HH:mm")));
        }
            
        await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("CHECK_PRAISES_TEMPLATE", user.Username, account.Praises.Count, praiseList.ToString()));
    }

    [Command("Achievements")]
    [Alias("Osiągnięcia", "Osiagniecia")]
    public async Task CheckAchievements(SocketGuildUser user = null)
    {
        if (user is null)
        {
            user = Context.User as SocketGuildUser;
        }
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);
        UserAccounts accounts = new UserAccounts(guild.Id, _storage);
        UserAccount account = accounts.GetUserAccount(user.Id);

        StringBuilder archievementsList = new StringBuilder();

        foreach (Achievement achievement in account.Achievements)
        {
            archievementsList.Append(alerts.GetFormattedAlert("PARSE_ACHIEVEMENT", achievement.Id, achievement.Value, achievement.Content));
        }

        await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("CHECK_ACHIEVEMENTS_TEMPLATE", user.Username, account.Achievements.Count,
                                                                        UserAccounts.GetAchievementsTotalPoints(account), archievementsList.ToString()));
    }

    [Command("Socials")]
    [Alias("Social Media")]
    public async Task GetSocials()
    {
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);

        EmbedBuilder embed = new EmbedBuilder()
        {
            Title = alerts.GetAlert("SOCIALS_TITLE"),
            Description = alerts.GetFormattedAlert("SOCIALS", Utility.DiscordInviteLink, Utility.TopGGPage, Utility.PatreonPage),
            ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl(),
            Color = new Color(1, 69, 44)
        };

        await Context.Channel.SendMessageAsync("", embed: embed.Build());
    }

    [Command("Credits")]
    [Alias("Twórcy", "Tworcy")]
    public async Task GetCredits()
    {
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);

        EmbedBuilder embed = new EmbedBuilder()
        {
            Title = alerts.GetAlert("CREDITS_TITLE"),
            Description = alerts.GetFormattedAlert("CREDITS", Utility.SaraArtistInstagram),
            ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl(),
            Color = new Color(1, 69, 44)
        };

        await Context.Channel.SendMessageAsync("", embed: embed.Build());
    }

    [Command("Help")]
    [Alias("Pomoc")]
    public async Task GetHelp()
    {
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);

        EmbedBuilder embed = new EmbedBuilder()
        {
            Title = alerts.GetAlert("HELP_TITLE"),
            Description = alerts.GetFormattedAlert("HELP", Utility.GitHubPage, Utility.DiscordInviteLink),
            ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl(),
            Color = new Color(1, 69, 44)
        };

        await Context.Channel.SendMessageAsync("", embed: embed.Build());
    }

    [Command("Show Level Roles", RunMode = RunMode.Async)]
    [Alias("Pokaż Role Za Level", "Pokaz Role Za Level")]
    public async Task ShowLevelRoles()
    {
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);

        IList<LevelRoleReward> roleRewards = new LevelRoleRewards(guild.Id, _storage).GetLevelRoleRewards();
        StringBuilder description = new StringBuilder();

        for (int i = 0; i < roleRewards.Count; i++)
        {
            description.Append(alerts.GetFormattedAlert("LEVEL_ROLE_LIST_ITEM", i + 1, roleRewards[i].RoleId, roleRewards[i].RequiredLevel));
        }

        EmbedBuilder embed = new EmbedBuilder()
        {
            Title = alerts.GetAlert("LEVEL_ROLE_LIST_TEMPLATE"),
            Description = description.ToString(),
            Color = new Color(1, 69, 44)
        };

        await Context.Channel.SendMessageAsync("", embed: embed.Build());
    }

    [Command("Vote")]
    [Alias("Głosowanie", "Glosowanie", "Propozycja")]
    public async Task Vote([Remainder]string msg = null)
    {
        Emoji yes = new Emoji("❤");
        Emoji wait = new Emoji("✋");
        Emoji no = new Emoji("👎");

        IEmote[] emotes = new IEmote[3] { yes, wait, no };

        await Context.Message.AddReactionsAsync(emotes);
    }

    [Command("Poll", RunMode = RunMode.Async)]
    [Alias("Ankieta")]
    public async Task Poll([Remainder] string msg)
    {
        PermissionUtils.TryRemoveMessageAsync(Context);

        string[] possibleAnswers = msg.Split('|');
        IEmote[] emotes = new IEmote[possibleAnswers.Length - 1];

        EmbedBuilder embed = new EmbedBuilder()
        {
            Title = possibleAnswers[0],
            Color = new Color(1, 69, 44)
        };

        embed.WithAuthor(Context.User);

        for (int i = 0; i < possibleAnswers.Length - 1; i++)
        {
            emotes[i] = Utility.AnswersEmojis.ToArray()[i];
            embed.AddField($"{emotes[i]}", $"**{possibleAnswers[i + 1]}**", true);
        }

        RestUserMessage pollMessage = await Context.Channel.SendMessageAsync("", embed: embed.Build());
        await pollMessage.AddReactionsAsync(emotes);
    }

    [Command("Leaderboard", RunMode = RunMode.Async)]
    [Alias("Ranking")]
    public async Task Leaderboard(int places = 15, char type = 'x')
    {
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);
        UserAccounts accounts = new UserAccounts(guild.Id, _storage);
        UserAccount account = accounts.GetUserAccount(Context.User.Id);

        IList<UserAccount> userAccounts = type switch
        {
            'a' => accounts.GetUserAccounts().OrderByDescending(u => u.Achievements.Sum(a => a.Value)).ToList(),
            'p' => accounts.GetUserAccounts().OrderByDescending(u => u.Praises.Count).ToList(),
            _ => accounts.GetUserAccounts().OrderByDescending(u => u.XP).ToList()
        };

        int index = userAccounts.IndexOf(account) + 1;

        StringBuilder msg = new StringBuilder();

        int usersInLeaderboard = Math.Min(places, userAccounts.Count); 

        for (int i = 0; i < usersInLeaderboard; i++)
        {
            msg.Append($"{i + 1}. <@{userAccounts[i].Id}>\n");
        }

        EmbedBuilder embed = new EmbedBuilder()
        {
            Title = alerts.GetFormattedAlert("LEADERBOARD_TITLE", guild.Name),
            Description = alerts.GetFormattedAlert("LEADERBOARD", msg.ToString(), index),
            Color = new Color(1, 69, 44)
        };

        await Context.Channel.SendMessageAsync("", embed: embed.Build());
    }

    [Command("Quote")]
    [Alias("Zacytuj", "Cytuj")]
    public async Task Quote(ulong msgId, ISocketMessageChannel channel = null)
    {
        if (channel is null)
        {
            channel = Context.Channel;
        }

        IMessage msg = await channel.GetMessageAsync(msgId);

        EmbedBuilder embed = new EmbedBuilder()
        {
            Description = $"{msg.Timestamp.DateTime}:\n {msg.Content}",
            Color = new Color(1, 69, 44)
        };
        embed.WithAuthor(msg.Author);

        await Context.Channel.SendMessageAsync("", embed: embed.Build());
    }

    [Command("Avatar")]
    [Alias("Awatar")]
    public async Task GetAvatar(SocketUser user = null)
    {
        if (user is null)
        {
            user = Context.User;
        }

        await Context.Channel.SendMessageAsync(user.GetAvatarUrl(ImageFormat.Auto, 2048));
    }

    [Command("Support Us")]
    [Alias("Wesprzyj Nas")]
    public async Task SupportUs()
    {
        SocketGuild guild = Context.Guild;
        GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
        AlertsHandler alerts = new AlertsHandler(config);

        EmbedBuilder embed = new EmbedBuilder()
        {
            Title = alerts.GetAlert("SUPPORT_US_TITLE"),
            Description = alerts.GetFormattedAlert("SUPPORT_US", Utility.DiscordInviteLink, Utility.TopGGPage, Utility.PatreonPage),
            ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl(),
            Color = new Color(1, 69, 44)
        };

        await Context.Channel.SendMessageAsync("", embed: embed.Build());
    }

    [Command("Any thoughts")]
    [RequireMotherServerMember]
    public async Task AnyThoughts()
    {
        string msg = Dialogue.GetRandomResponse(Context);
        await Context.Channel.SendMessageAsync(msg);
    }

    private static string GetUserWarningsMessage(SocketGuildUser user, GuildConfig config, AlertsHandler alerts, UserAccount account)
    {
        StringBuilder warnList = new StringBuilder();

        if (config.PointBasedWarns)
        {
            foreach (Warning warn in account.Warnings)
            {
                warnList.Append(alerts.GetFormattedAlert("PARSE_WARN_POINT", warn.Id, warn.Value, warn.Reason, warn.ExpireDate.ToString(@"dd\/MM\/yyyy HH:mm")));
            }

            return alerts.GetFormattedAlert("CHECK_WARNINGS_TEMPLATE_POINT", user.Username, account.Warnings.Count, UserAccounts.GetWarningsPower(account), warnList.ToString());
        }

        foreach (Warning warn in account.Warnings)
        {
            warnList.Append(alerts.GetFormattedAlert("PARSE_WARN", warn.Id, warn.Reason, warn.ExpireDate.ToString(@"dd\/MM\/yyyy HH:mm")));
        }

        return alerts.GetFormattedAlert("CHECK_WARNINGS_TEMPLATE", user.Username, account.Warnings.Count, warnList.ToString());
    }
}
