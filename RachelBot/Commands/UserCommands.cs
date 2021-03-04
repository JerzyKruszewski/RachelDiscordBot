using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace RachelBot.Commands
{
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
            if (user == null)
            {
                user = Context.User as SocketGuildUser;
            }
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            UserAccounts accounts = new UserAccounts(guild.Id, _storage);
            UserAccount account = accounts.GetUserAccount(user.Id);

            accounts.DeleteExpiredWarnings(account);

            string message;

            if (config.PointBasedWarns)
            {
                message = alerts.GetFormattedAlert("STATUS_MESSAGE_POINT", user.Username, account.LevelNumber, account.XP, account.Praises.Count, account.Archievements.Count,
                                                                           account.Warnings.Count, accounts.GetWarningsPower(account));
            }
            else
            {
                message = alerts.GetFormattedAlert("STATUS_MESSAGE", user.Username, account.LevelNumber, account.XP, account.Praises.Count, account.Archievements.Count,
                                                                     account.Warnings.Count);
            }

            await Context.Channel.SendMessageAsync(message);
        }

        [Command("Warns")]
        public async Task CheckWarns(SocketGuildUser user = null)
        {
            if (user == null)
            {
                user = Context.User as SocketGuildUser;
            }
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            UserAccounts accounts = new UserAccounts(guild.Id, _storage);
            UserAccount account = accounts.GetUserAccount(user.Id);

            accounts.DeleteExpiredWarnings(account);

            string message;
            string warnList = "";

            if (config.PointBasedWarns)
            {
                foreach (Warning warn in account.Warnings)
                {
                    warnList += alerts.GetFormattedAlert("PARSE_WARN_POINT", warn.Id, warn.Value, warn.Reason, warn.ExpireDate.ToString(@"dd\/MM\/yyyy HH:mm"));
                }
                message = alerts.GetFormattedAlert("CHECK_WARNINGS_TEMPLATE_POINT", user.Username, account.Warnings.Count, accounts.GetWarningsPower(account), warnList);
            }
            else
            {
                foreach (Warning warn in account.Warnings)
                {
                    warnList += alerts.GetFormattedAlert("PARSE_WARN", warn.Id, warn.Reason, warn.ExpireDate.ToString(@"dd\/MM\/yyyy HH:mm"));
                }
                message = alerts.GetFormattedAlert("CHECK_WARNINGS_TEMPLATE", user.Username, account.Warnings.Count, warnList);
            }

            await Context.Channel.SendMessageAsync(message);
        }

        [Command("Praises")]
        public async Task CheckPraises(SocketGuildUser user = null)
        {
            if (user == null)
            {
                user = Context.User as SocketGuildUser;
            }
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            UserAccounts accounts = new UserAccounts(guild.Id, _storage);
            UserAccount account = accounts.GetUserAccount(user.Id);

            string praiseList = "";

            foreach (Praise praise in account.Praises)
            {
                praiseList += alerts.GetFormattedAlert("PARSE_PRAISE", praise.Id, praise.Reason, praise.GivenAt.ToString(@"dd\/MM\/yyyy HH:mm"));
            }
            
            await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("CHECK_PRAISES_TEMPLATE", user.Username, account.Praises.Count, praiseList));
        }

        [Command("Archievements")]
        public async Task CheckArchievements(SocketGuildUser user = null)
        {
            if (user == null)
            {
                user = Context.User as SocketGuildUser;
            }
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            UserAccounts accounts = new UserAccounts(guild.Id, _storage);
            UserAccount account = accounts.GetUserAccount(user.Id);

            string archievementsList = "";

            for (int i = 1; i <= account.Archievements.Count; i++)
            {
                archievementsList += alerts.GetFormattedAlert("PARSE_ARCHIEVEMENT", i, account.Archievements[i]);
            }

            await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("CHECK_ARCHIEVEMENTS_TEMPLATE", user.Username, account.Archievements.Count, archievementsList));
        }

        [Command("Socials")]
        public async Task GetSocials()
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = alerts.GetAlert("SOCIALS_TITLE"),
                Description = alerts.GetFormattedAlert("SOCIALS", Utility.DiscordInviteLink),
                ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                Color = new Color(1, 69, 44)
            };

            await Context.Channel.SendMessageAsync("", embed: embed.Build());
        }

        [Command("Credits")]
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
    }
}
