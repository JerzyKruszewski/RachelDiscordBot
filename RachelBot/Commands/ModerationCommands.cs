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
using Discord.Rest;
using RachelBot.Core.ModerationAnnouncements;

namespace RachelBot.Commands
{
    [RequirePublicChannel]
    public class ModerationCommands : InteractiveBase<SocketCommandContext>
    {
        private readonly IStorageService _storage;

        public ModerationCommands(IStorageService storage)
        {
            _storage = storage;
        }

        [Command("Ban")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUser(SocketGuildUser user, [Remainder]string reason)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;

            await user.BanAsync(0, reason);

            await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_BANNED", user.Username, user.Id, reason));
        }

        [Command("Kick")]
        [Alias("Wyrzuć", "Wyrzuc")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUser(SocketGuildUser user, [Remainder]string reason)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;

            await user.KickAsync(reason);

            await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_KICKED", user.Username, user.Id, reason));
        }

        [Command("Praise")]
        [Alias("Pochwal")]
        [RequireStaff]
        public async Task PraiseUser(SocketGuildUser user, [Remainder]string reason)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel channel = Context.Channel;

            if (user == Context.User)
            {
                await channel.SendMessageAsync(alerts.GetAlert("SELF_PRAISE_ABUSE"));
                return;
            }

            UserAccounts accounts = new UserAccounts(Context.Guild.Id, _storage);

            accounts.AddPraise(accounts.GetUserAccount(user.Id), Context.User.Id, reason);

            await channel.SendMessageAsync(alerts.GetFormattedAlert("USER_PRAISED", user.Mention, Context.User.Mention, reason));
        }

        [Command("Remove Praise")]
        [Alias("Usuń pochwałę", "Usun pochwale")]
        [RequireStaff]
        public async Task RemovePraise(SocketGuildUser user, int id)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel channel = Context.Channel;

            UserAccounts accounts = new UserAccounts(Context.Guild.Id, _storage);

            accounts.RemovePraise(accounts.GetUserAccount(user.Id), id);

            await channel.SendMessageAsync(alerts.GetFormattedAlert("PRAISE_REMOVED", id, user.Mention));
        }

        [Command("Reprimand", RunMode = RunMode.Async)]
        [Alias("Upomnienie")]
        [RequireStaff]
        public async Task RebukeUser(SocketGuildUser user, [Remainder] string reason)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;
            string message;

            try
            {
                IDMChannel dmChannel = await user.GetOrCreateDMChannelAsync();

                message = alerts.GetFormattedAlert("USER_REPRIMANDED_MESSAGE", user.Mention, guild.Name, reason, config.ToSChannelId);

                await dmChannel.SendMessageAsync(message);
                await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_REPRIMANDED", user.Mention, reason, message));
            }
            catch (Exception)
            {
                await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_HAS_CLOSED_DMS", user.Mention));
            }
        }

        [Command("Warn", RunMode = RunMode.Async)]
        [Alias("Ostrzeżenie", "Ostrzezenie")]
        [RequireStaff]
        public async Task WarnUser(SocketGuildUser user, [Remainder] string reason)
        {
            try
            {
                SocketGuild guild = Context.Guild;
                GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
                AlertsHandler alerts = new AlertsHandler(config);
                ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;
                UserAccounts accounts = new UserAccounts(Context.Guild.Id, _storage);
                UserAccount account = accounts.GetUserAccount(user.Id);

                accounts.DeleteExpiredWarnings(account);
                Warning warn = accounts.AddWarning(account, reason, config);

                reason = Utility.ParseReason(reason, config);

                if ((account.Warnings.Count >= config.WarnCountTillBan && config.WarnCountTillBan > 0) ||
                    (account.Warnings.Sum(w => w.Value) >= config.WarnPointsTillBan && config.WarnPointsTillBan > 0))
                {
                    if (PermissionUtils.CanBanMembers(Context))
                    {
                        await user.BanAsync(0, alerts.GetFormattedAlert("TOO_MANY_WARNS", reason));

                        await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_BANNED", user.Username, user.Id, alerts.GetFormattedAlert("TOO_MANY_WARNS", reason)));

                        return;
                    }
                }
                else if ((account.Warnings.Count >= config.WarnCountTillPunishment && config.WarnCountTillPunishment > 0) ||
                         (account.Warnings.Sum(w => w.Value) >= config.WarnPointsTillPunishment && config.WarnPointsTillPunishment > 0))
                {
                    if (PermissionUtils.CanGiveRoles(Context))
                    {
                        await user.AddRoleAsync(Utility.GetRoleById(Context.Guild, config.PunishmentRoleId));

                        await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_GOT_PUNISHMENT_ROLE", user.Mention, config.PunishmentRoleId,
                                                                                   alerts.GetFormattedAlert("TOO_MANY_WARNS", reason)));
                    }
                }

                string message;

                try
                {
                    IDMChannel dmChannel = await user.GetOrCreateDMChannelAsync();

                    if (config.PointBasedWarns)
                    {
                        message = alerts.GetFormattedAlert("USER_WARNED_MESSAGE_POINT", user.Mention, guild.Name, warn.Value, warn.Reason,
                                                           accounts.GetWarningsCount(account), accounts.GetWarningsPower(account), config.ToSChannelId);

                        await dmChannel.SendMessageAsync(message);
                        await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_WARNED_POINT", user.Mention, reason, warn.Value, message));
                    }
                    else
                    {
                        message = alerts.GetFormattedAlert("USER_WARNED_MESSAGE", user.Mention, guild.Name, warn.Reason,
                                                           accounts.GetWarningsCount(account), config.ToSChannelId);

                        await dmChannel.SendMessageAsync(message);
                        await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_WARNED", user.Mention, reason, message));
                    }
                }
                catch (Exception)
                {
                    await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_HAS_CLOSED_DMS_WARN", user.Mention));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Program.LogToFile($"{ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        [Command("Remove Warn", RunMode = RunMode.Async)]
        [Alias("Usuń Ostrzeżenie", "Usun Ostrzezenie")]
        [RequireStaff]
        public async Task WarnUser(SocketGuildUser user, int warnId)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;
            UserAccounts accounts = new UserAccounts(Context.Guild.Id, _storage);
            UserAccount account = accounts.GetUserAccount(user.Id);

            accounts.DeleteExpiredWarnings(account);

            try
            {
                accounts.RemoveWarning(account, warnId);
            }
            catch (Exception)
            {
                await modChannel.SendMessageAsync(alerts.GetAlert("WARN_REMOVE_FAIL"));
                return;
            }

            await modChannel.SendMessageAsync(alerts.GetAlert("WARN_REMOVE_SUCCESS"));
        }

        [Command("Achievement")]
        [Alias("Osiągnięcie", "Osiagniecie")]
        [RequireStaff]
        public async Task Achievement(SocketGuildUser user, int value, [Remainder]string achievement)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;

            if (user == Context.User)
            {
                await Context.Channel.SendMessageAsync(alerts.GetAlert("SELF_ACHIEVEMENT_ABUSE"));
                return;
            }

            UserAccounts accounts = new UserAccounts(Context.Guild.Id, _storage);

            accounts.AddAchievement(accounts.GetUserAccount(user.Id), value, achievement);

            await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_GOT_ACHIEVEMENT", user.Mention, achievement));
        }

        [Command("Remove Achievement")]
        [Alias("Usuń Osiągnięcie", "Usun Osiagniecie")]
        [RequireStaff]
        public async Task RemoveAchievement(SocketGuildUser user, int id)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;

            UserAccounts accounts = new UserAccounts(Context.Guild.Id, _storage);

            accounts.RemoveAchievement(accounts.GetUserAccount(user.Id), id);

            await modChannel.SendMessageAsync(alerts.GetFormattedAlert("ACHIEVEMENT_REMOVED", id, user.Mention));
        }

        [Command("Unlock Channel")]
        [Alias("Odblokuj Kanał", "Odblokuj Kanal")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task UnlockChannels(IRole role, params IGuildChannel[] channels)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;

            OverwritePermissions permissions = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow, embedLinks: PermValue.Allow, 
                attachFiles: PermValue.Allow, readMessageHistory: PermValue.Allow, mentionEveryone: PermValue.Deny, useExternalEmojis: PermValue.Allow);

            foreach (IGuildChannel channel in channels)
            {
                await channel.AddPermissionOverwriteAsync(role, permissions);
            }

            await modChannel.SendMessageAsync(alerts.GetAlert("CHANNELS_UNLOCKED"));
        }

        [Command("Lock Channel")]
        [Alias("Zablokuj Kanał", "Zablokuj Kanal")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task LockChannels(IRole role, params IGuildChannel[] channels)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;

            OverwritePermissions permissions = new OverwritePermissions(viewChannel: PermValue.Deny, sendMessages: PermValue.Deny, embedLinks: PermValue.Deny, 
                attachFiles: PermValue.Deny, readMessageHistory: PermValue.Deny, mentionEveryone: PermValue.Deny, useExternalEmojis: PermValue.Deny);

            foreach (IGuildChannel channel in channels)
            {
                await channel.AddPermissionOverwriteAsync(role, permissions);
            }

            await modChannel.SendMessageAsync(alerts.GetAlert("CHANNELS_LOCKED"));
        }

        [Command("Add Level Role")]
        [Alias("Dodaj Rolę Za Level", "Dodaj Role Za Level")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddLevelRole(IRole role, uint level)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;

            new LevelRoleRewards(Context.Guild.Id, _storage).CreateLevelRoleReward(role.Id, level);

            await modChannel.SendMessageAsync(alerts.GetFormattedAlert("LEVEL_ROLE_ADDED", role.Id, level));
        }

        [Command("Remove Level Role")]
        [Alias("Usuń Rolę Za Level", "Usun Role Za Level")]
        [RequireStaff]
        public async Task RemoveLevelRole(IRole role)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;

            new LevelRoleRewards(Context.Guild.Id, _storage).RemoveLevelRoleReward(role.Id);

            await modChannel.SendMessageAsync(alerts.GetFormattedAlert("LEVEL_ROLE_REMOVED", role.Id));
        }

        [Command("Create Announcement", RunMode = RunMode.Async)]
        [Alias("Nowe Ogłoszenie", "Nowe Ogloszenie")]
        [RequireStaff]
        public async Task CreateAnnouncement(ITextChannel channel, [Remainder]string content)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            Announcements announcements = new Announcements(guild.Id, _storage);

            Tuple<string, string> titleAndContent = SplitAnnouncement(content);
            string title = titleAndContent.Item1;
            content = titleAndContent.Item2;

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = title,
                Description = content,
                Color = new Color(1, 69, 44)
            };

            embed.WithAuthor(Context.User);

            IUserMessage message = await channel.SendMessageAsync(embed: embed.Build());

            announcements.CreateAnnouncement(message.Id, channel.Id, content);

            await Task.Delay(500);

            await Context.Channel.SendMessageAsync(alerts.GetAlert("SUCCESS"));
        }

        [Command("Update Announcement")]
        [Alias("Zaktualizuj Ogłoszenie", "Zaktualizuj Ogloszenie")]
        [RequireStaff]
        public async Task UpdateAnnouncement(ulong messageId, [Remainder] string content)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            Announcements announcements = new Announcements(guild.Id, _storage);

            Announcement announcement = announcements.UpdateAnnouncement(messageId, content);

            if (announcement is null)
            {
                await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("NO_ANNOUNCEMENT", messageId));
                return;
            }

            Tuple<string, string> titleAndContent = SplitAnnouncement(content);
            string title = titleAndContent.Item1;
            content = titleAndContent.Item2;

            IMessage message = await Utility.GetMessageChannelById(guild, announcement.ChannelId).GetMessageAsync(messageId);

            if (message is null)
            {
                await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("NO_ANNOUNCEMENT", messageId));
                return;
            }

            await (message as RestUserMessage).ModifyAsync(m => 
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = title,
                    Description = content,
                    Color = new Color(1, 69, 44)
                };

                embed.WithAuthor(Context.User);

                m.Embed = embed.Build();
            });

            await Context.Channel.SendMessageAsync(alerts.GetAlert("SUCCESS"));
        }

        private static Tuple<string, string> SplitAnnouncement(string content)
        {
            string[] split = content.Split('|');

            string title = split[0];

            content = "";

            for (int i = 1; i < split.Length; i++)
            {
                content += $"{split[i]}|";
            }

            content = content.Remove(content.Length - 1);

            return Tuple.Create(title, content);
        }

        [Command("Slowmode", RunMode = RunMode.Async)]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task Slowmode(int interval, params SocketTextChannel[] channels)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            
            foreach (SocketTextChannel channel in channels)
            {
                await channel.ModifyAsync(c =>
                {
                    c.SlowModeInterval = interval;
                });

                await Task.Delay(250);
            }

            await Context.Channel.SendMessageAsync(alerts.GetAlert("SUCCESS"));
        }

        [Command("Give Role")]
        [Alias("Daj Rolę", "Daj Role")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task GiveRole(SocketRole role, SocketGuildUser user = null)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);

            if (user is null)
            {
                user = Context.User as SocketGuildUser;
            }

            await user.AddRoleAsync(role);

            await Context.Channel.SendMessageAsync(alerts.GetAlert("SUCCESS"));
        }

        [Command("Remove Role")]
        [Alias("Usuń Rolę", "Usun Role")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task RemoveRole(SocketRole role, SocketGuildUser user = null)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);

            if (user is null)
            {
                user = Context.User as SocketGuildUser;
            }

            await user.RemoveRoleAsync(role);

            await Context.Channel.SendMessageAsync(alerts.GetAlert("SUCCESS"));
        }
    }
}
