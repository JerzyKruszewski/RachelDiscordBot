using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Rest;
using Discord.Addons.Interactive;
using RachelBot.Core.Configs;
using RachelBot.Services.Storage;
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
                message = alerts.GetFormattedAlert("STATUS_MESSAGE_POINT", user.Username, account.LevelNumber, account.XP, account.Praises.Count, account.Achievements.Count,
                                                                           account.Warnings.Count, accounts.GetWarningsPower(account));
            }
            else
            {
                message = alerts.GetFormattedAlert("STATUS_MESSAGE", user.Username, account.LevelNumber, account.XP, account.Praises.Count, account.Achievements.Count,
                                                                     account.Warnings.Count);
            }

            await Context.Channel.SendMessageAsync(message);
        }

        [Command("Warns")]
        [Alias("Ostrzeżenia", "Ostrzezenia")]
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
        [Alias("Pochwały", "Pochwaly")]
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

        [Command("Achievements")]
        [Alias("Osiągnięcia", "Osiagniecia")]
        public async Task CheckAchievements(SocketGuildUser user = null)
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

            foreach (Achievement achievement in account.Achievements)
            {
                archievementsList += alerts.GetFormattedAlert("PARSE_ACHIEVEMENT", achievement.Id, achievement.Value, achievement.Content);
            }

            await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("CHECK_ACHIEVEMENTS_TEMPLATE", user.Username, account.Achievements.Count,
                                                                            accounts.GetAchievementsTotalPoints(account), archievementsList));
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
                Description = alerts.GetFormattedAlert("SOCIALS", Utility.DiscordInviteLink),
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
            string description = "";

            for (int i = 0; i < roleRewards.Count; i++)
            {
                description += alerts.GetFormattedAlert("LEVEL_ROLE_LIST_ITEM", i + 1, roleRewards[i].RoleId, roleRewards[i].RequiredLevel);
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = alerts.GetAlert("LEVEL_ROLE_LIST_TEMPLATE"),
                Description = description,
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
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Poll([Remainder] string msg)
        {
            await Context.Message.DeleteAsync();

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
            IList<UserAccount> userAccounts;

            switch (type)
            {
                case 'a':
                    userAccounts = accounts.GetUserAccounts().OrderByDescending(u => u.Achievements.Sum(a => a.Value)).ToList();
                    break;
                case 'p':
                    userAccounts = accounts.GetUserAccounts().OrderByDescending(u => u.Praises.Count).ToList();
                    break;
                default:
                    userAccounts = accounts.GetUserAccounts().OrderByDescending(u => u.XP).ToList();
                    break;
            }

            int index = userAccounts.IndexOf(account) + 1;

            string msg = "";

            for (int i = 0; i < places; i++)
            {
                msg += $"{i + 1}. <@{userAccounts[i].Id}>\n";
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = alerts.GetFormattedAlert("LEADERBOARD_TITLE", guild.Name),
                Description = alerts.GetFormattedAlert("LEADERBOARD", msg, index),
                Color = new Color(1, 69, 44)
            };

            await Context.Channel.SendMessageAsync("", embed: embed.Build());
        }

        [Command("Quote")]
        [Alias("Zacytuj", "Cytuj")]
        public async Task Quote(ulong msgId, ISocketMessageChannel channel = null)
        {
            if (channel == null)
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
            if (user == null)
            {
                user = Context.User;
            }

            await Context.Channel.SendMessageAsync(user.GetAvatarUrl(ImageFormat.Auto, 2048));
        }
    }
}
