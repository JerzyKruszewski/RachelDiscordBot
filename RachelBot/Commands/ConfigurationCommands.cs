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

namespace RachelBot.Commands
{
    [Group("Config")]
    public class ConfigurationCommands : InteractiveBase<SocketCommandContext>
    {
        private readonly IStorageService _storage;

        public ConfigurationCommands(IStorageService storage)
        {
            _storage = storage;
        }

        [Command("GuildPrefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeGuildPrefix(string prefix)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeGuildPrefix(prefix);
        }

        [Command("ChangeGuildLanguage")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeGuildLanguage(string languageIso)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeGuildLanguage(languageIso);
        }

        [Command("AddStaffRoles")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddStaffRoles(params SocketRole[] staffRoles)
        {
            List<ulong> list = new List<ulong>();
            
            foreach (SocketRole role in staffRoles)
            {
                list.Add(role.Id);
            }

            new GuildConfigs(Context.Guild.Id, _storage).AddStaffRoles(list);
        }

        [Command("ChangeStaffRoles")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeStaffRoles(params SocketRole[] staffRoles)
        {
            List<ulong> list = new List<ulong>();

            foreach (SocketRole role in staffRoles)
            {
                list.Add(role.Id);
            }

            new GuildConfigs(Context.Guild.Id, _storage).ChangeStaffRoles(list);
        }

        [Command("ChangeGuildModerationChannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeGuildModerationChannel(ITextChannel channel)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeGuildModerationChannel(channel.Id);
        }

        [Command("ChangeUsersJoiningChannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeUsersJoiningChannel(ITextChannel channel)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeUsersJoiningChannel(channel.Id);
        }

        [Command("ChangeWelcomeMessage")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeWelcomeMessage([Remainder]string message)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeWelcomeMessage(message);
        }

        [Command("ChangeUsersLeftChannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeUsersLeftChannel(ITextChannel channel)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeUsersLeftChannel(channel.Id);
        }

        [Command("ChangeUserLeftMessage")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeUserLeftMessage([Remainder]string message)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeUserLeftMessage(message);
        }

        [Command("ChangePunishmentRole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangePunishmentRole(IRole role)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangePunishmentRole(role.Id);
        }

        [Command("ChangePunishmentChannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangePunishmentChannel(ITextChannel channel)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangePunishmentChannel(channel.Id);
        }

        [Command("TogglePointSystemWarns")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task TogglePointSystemWarns(bool toggle)
        {
            new GuildConfigs(Context.Guild.Id, _storage).TogglePointSystemWarns(toggle);
        }

        [Command("ChangeWarnDuration")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeWarnDuration(uint duration)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeWarnDuration(duration);
        }

        [Command("ChangeWarnCountTillBan")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeWarnCountTillBan(uint warnCount)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeWarnCountTillBan(warnCount);
        }

        [Command("ChangeWarnCountTillPunishment")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeWarnCountTillPunishment(uint warnCount)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeWarnCountTillPunishment(warnCount);
        }

        [Command("ChangeWarnPointsTillBan")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeWarnPointsTillBan(uint warnCount)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeWarnPointsTillBan(warnCount);
        }

        [Command("ChangeWarnPointsTillPunishment")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeWarnPointsTillPunishment(uint warnCount)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeWarnPointsTillPunishment(warnCount);
        }

        [Command("ChangeAnnouncementChannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeAnnouncementChannel(ITextChannel channel)
        {
            new GuildConfigs(Context.Guild.Id, _storage).ChangeAnnouncementChannel(channel.Id);
        }
    }
}
