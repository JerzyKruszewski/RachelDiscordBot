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
    public class ModerationCommands : InteractiveBase<SocketCommandContext>
    {
        private readonly IStorageService _storage;

        public ModerationCommands(IStorageService storage)
        {
            _storage = storage;
        }

        [Command("Ban")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
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
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
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
        [RequireBotPermission(GuildPermission.Administrator)]
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

        [Command("Warn", RunMode = RunMode.Async)]
        [Alias("Ostrzeżenie")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task WarnUser(SocketGuildUser user, [Remainder] string reason)
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
                await user.BanAsync(0, alerts.GetFormattedAlert("TOO_MANY_WARNS", reason));

                await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_BANNED", user.Username, user.Id, alerts.GetFormattedAlert("TOO_MANY_WARNS", reason)));

                return;
            }
            else if ((account.Warnings.Count >= config.WarnCountTillPunishment && config.WarnCountTillPunishment > 0) ||
                     (account.Warnings.Sum(w => w.Value) >= config.WarnPointsTillPunishment && config.WarnPointsTillPunishment > 0))
            {
                await user.AddRoleAsync(Utility.GetRoleById(Context.Guild, config.PunishmentRoleId));

                await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_GOT_PUNISHMENT_ROLE", user.Mention, config.PunishmentRoleId, 
                                                                           alerts.GetFormattedAlert("TOO_MANY_WARNS", reason)));
            }

            string message;

            try
            {
                IDMChannel dmChannel = await user.GetOrCreateDMChannelAsync();

                if (config.PointBasedWarns)
                {
                    message = alerts.GetFormattedAlert("USER_WARNED_MESSAGE_POINT", user.Mention, warn.Value, warn.Reason,
                                                       accounts.GetWarningsCount(account), accounts.GetWarningsPower(account), config.ToSChannelId);

                    await dmChannel.SendMessageAsync(message);
                    await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_WARNED_POINT", user.Mention, reason, warn.Value, message));
                }
                else
                {
                    message = alerts.GetFormattedAlert("USER_WARNED_MESSAGE", user.Mention, warn.Reason,
                                                       accounts.GetWarningsCount(account), config.ToSChannelId);

                    await dmChannel.SendMessageAsync(message);
                    await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_WARNED", user.Mention, reason, message));
                }
            }
            catch (Exception)
            {
                await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_HAS_CLOSED_DMS", user.Mention));
            }
        }

        [Command("Remove Warn", RunMode = RunMode.Async)]
        [Alias("Usuń Ostrzeżenie")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
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

        [Command("Archievement")]
        [Alias("Osiągnięcie")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task Archievement(SocketGuildUser user, [Remainder]string archievement)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;

            if (user == Context.User)
            {
                await Context.Channel.SendMessageAsync(alerts.GetAlert("SELF_ARCHIEVEMENT_ABUSE"));
                return;
            }

            UserAccounts accounts = new UserAccounts(Context.Guild.Id, _storage);

            accounts.AddAchievement(accounts.GetUserAccount(user.Id), archievement);

            await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_GOT_ARCHIEVEMENT", user.Mention, archievement));
        }

        [Command("Unlock Channel")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
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
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
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
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
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
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task RemoveLevelRole(IRole role)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;

            new LevelRoleRewards(Context.Guild.Id, _storage).RemoveLevelRoleReward(role.Id);

            await modChannel.SendMessageAsync(alerts.GetFormattedAlert("LEVEL_ROLE_REMOVED", role.Id));
        }

        [Command("Show Level Roles", RunMode = RunMode.Async)]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task ShowLevelRoles()
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;

            IList<LevelRoleReward> roleRewards = new LevelRoleRewards(Context.Guild.Id, _storage).GetLevelRoleRewards();
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

            await modChannel.SendMessageAsync("", embed: embed.Build());
        }
    }
}
