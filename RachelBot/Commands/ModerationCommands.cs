﻿using System;
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
    [Group("Staff")]
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

        //TODO:
        //Add message to user
        [Command("Warn")]
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

            if (config.PointBasedWarns)
            {
                await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_WARNED_POINT", user.Mention, reason, warn.Value, "PLACEHOLDER"));
            }
            else
            {
                await modChannel.SendMessageAsync(alerts.GetFormattedAlert("USER_WARNED", user.Mention, reason, "PLACEHOLDER"));
            }
        }

        [Command("Archievement")]
        [Alias("Osiągnięcie")]
        [RequireStaff]
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

        [Command("unlock channel")]
        [RequireStaff]
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

        [Command("lock channel")]
        [RequireStaff]
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

        [Command("Add level role")]
        [RequireStaff]
        public async Task AddLevelRole(IRole role, uint level)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            ISocketMessageChannel modChannel = Utility.GetMessageChannelById(guild, config.ModeratorChannelId) ?? Context.Channel;

            new LevelRoleRewards(Context.Guild.Id, _storage).CreateLevelRoleReward(role.Id, level);

            await modChannel.SendMessageAsync(alerts.GetFormattedAlert("LEVEL_ROLE_ADDED", role.Id, level));
        }
    }
}
