using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using RachelBot.Core.Configs;
using RachelBot.Services.Storage;
using RachelBot.Lang;
using RachelBot.Preconditions;
using RachelBot.Core.StaffRoles;

namespace RachelBot.Commands;

[RequirePublicChannel]
public class ConfigurationCommands : InteractiveBase<SocketCommandContext>
{
    private readonly IStorageService _storage;

    public ConfigurationCommands(IStorageService storage)
    {
        _storage = storage;
    }

    [Command("ChangeGuildPrefix")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeGuildPrefix([Remainder]string prefix)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        if (Regex.IsMatch(prefix, @"<@[0-9&!]+>")) //User or role ping
        {
            await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("FAILURE"));
            return;
        }

        configs.ChangeGuildPrefix(prefix);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeGuildLanguage")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeGuildLanguage(string languageIso)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeGuildLanguage(languageIso);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("AddOrChangeStaffRole")]
    [Alias("AddStaffRole", "ChangeStaffRole")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task AddStaffRoles(SocketRole staffRole, int permType)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.AddOrChangeStaffRole(new StaffRole()
        {
            Id = staffRole.Id,
            PermissionType = (StaffPermissionType)permType,
        });

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ClearStaffRoles")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ClearStaffRoles()
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ClearStaffRoles();

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeModerationChannel")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeGuildModerationChannel(ITextChannel channel)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeGuildModerationChannel(channel.Id);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeUsersJoiningChannel")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeUsersJoiningChannel(ITextChannel channel)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeUsersJoiningChannel(channel.Id);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeWelcomeMessage")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeWelcomeMessage([Remainder]string message)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeWelcomeMessage(message);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeUsersLeftChannel")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeUsersLeftChannel(ITextChannel channel)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeUsersLeftChannel(channel.Id);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeUserLeftMessage")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeUserLeftMessage([Remainder]string message)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeUserLeftMessage(message);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangePunishmentRole")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangePunishmentRole(IRole role)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangePunishmentRole(role.Id);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangePunishmentChannel")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangePunishmentChannel(ITextChannel channel)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangePunishmentChannel(channel.Id);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("TogglePointSystemWarns")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task TogglePointSystemWarns(bool toggle)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.TogglePointSystemWarns(toggle);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeWarnDuration")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeWarnDuration(uint duration)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeWarnDuration(duration);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeWarnCountTillBan")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeWarnCountTillBan(uint warnCount)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeWarnCountTillBan(warnCount);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeWarnCountTillPunishment")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeWarnCountTillPunishment(uint warnCount)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeWarnCountTillPunishment(warnCount);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeWarnPointsTillBan")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeWarnPointsTillBan(uint warnCount)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeWarnPointsTillBan(warnCount);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeWarnPointsTillPunishment")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeWarnPointsTillPunishment(uint warnCount)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeWarnPointsTillPunishment(warnCount);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeAnnouncementChannel")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeAnnouncementChannel(ITextChannel channel)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeAnnouncementChannel(channel.Id);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ChangeToSChannel")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ChangeToSChannel(ITextChannel channel)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ChangeToSChannel(channel.Id);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("ToggleReactionToBotMessages")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ToggleReactionToBotMessages(bool toggle)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.ToggleReactionToBotMessages(toggle);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }

    [Command("TogglePhishingProtection")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task TogglePhishingProtection(bool toggle)
    {
        GuildConfigs configs = new GuildConfigs(Context.Guild.Id, _storage);

        configs.TogglePhishingProtection(toggle);

        await Context.Channel.SendMessageAsync(new AlertsHandler(configs.GetGuildConfig()).GetAlert("SUCCESS"));
    }
}
