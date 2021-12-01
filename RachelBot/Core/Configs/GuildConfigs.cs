using System.Configuration;
using RachelBot.Core.StaffRoles;
using RachelBot.Services.Storage;

namespace RachelBot.Core.Configs;

public class GuildConfigs
{
    private readonly IStorageService _storage;
    private readonly string _folderPath;
    private readonly string _filePath;
    private readonly GuildConfig _config;

    public GuildConfigs(ulong id, IStorageService storage)
    {
        _storage = storage;
        _folderPath = $"./Guilds/{id}";

        _storage.EnsureDirectoryExist(_folderPath);

        _filePath = $"{_folderPath}/Config.json";

        if (_storage.FileExist(_filePath))
        {
            _config = _storage.RestoreObject<GuildConfig>(_filePath);

            return;
        }

        _config = new GuildConfig()
        {
            GuildId = id,
            GuildPrefix = ConfigurationManager.AppSettings["Prefix"],
            WelcomeMessage = ConfigurationManager.AppSettings["UserJoinedMessage"],
            LeftMessage = ConfigurationManager.AppSettings["UserLeftMessage"]
        };

        Save();
    }

    private void Save()
    {
        _storage.StoreObject(_config, _filePath);
    }

    public GuildConfig GetGuildConfig()
    {
        return _config;
    }

    public GuildConfig ChangeGuildPrefix(string prefix)
    {
        _config.GuildPrefix = prefix;

        Save();

        return _config;
    }

    public GuildConfig ChangeGuildLanguage(string languageIso)
    {
        _config.GuildLanguageIso = languageIso;

        Save();

        return _config;
    }

    public GuildConfig AddOrChangeStaffRole(StaffRole role)
    {
        StaffRole staffRole = _config.StaffRoles.SingleOrDefault(r => r.Id == role.Id);

        if (staffRole is not null)
        {
            staffRole.PermissionType = role.PermissionType;
            Save();

            return _config;
        }

        _config.StaffRoles.Add(role);

        Save();

        return _config;
    }

    public GuildConfig ClearStaffRoles()
    {
        _config.StaffRoles.Clear();

        Save();

        return _config;
    }

    public GuildConfig ChangeGuildModerationChannel(ulong id)
    {
        _config.ModeratorChannelId = id;

        Save();

        return _config;
    }

    public GuildConfig ChangeUsersJoiningChannel(ulong id)
    {
        _config.InChannelId = id;

        Save();

        return _config;
    }

    public GuildConfig ChangeWelcomeMessage(string welcomeMessage)
    {
        _config.WelcomeMessage = welcomeMessage;

        Save();

        return _config;
    }

    public GuildConfig ChangeUsersLeftChannel(ulong id)
    {
        _config.OutChannelId = id;

        Save();

        return _config;
    }

    public GuildConfig ChangeUserLeftMessage(string leftMessage)
    {
        _config.LeftMessage = leftMessage;

        Save();

        return _config;
    }

    public GuildConfig ChangePunishmentRole(ulong id)
    {
        _config.PunishmentRoleId = id;

        Save();

        return _config;
    }

    public GuildConfig ChangePunishmentChannel(ulong id)
    {
        _config.PunishmentChannelId = id;

        Save();

        return _config;
    }

    public GuildConfig TogglePointSystemWarns(bool toggle)
    {
        _config.PointBasedWarns = toggle;

        Save();

        return _config;
    }

    public GuildConfig ChangeWarnDuration(uint days)
    {
        _config.WarnDuration = days;

        Save();

        return _config;
    }

    public GuildConfig ChangeWarnCountTillBan(uint warnCount)
    {
        _config.WarnCountTillBan = warnCount;

        Save();

        return _config;
    }

    public GuildConfig ChangeWarnCountTillPunishment(uint warnCount)
    {
        _config.WarnCountTillPunishment = warnCount;

        Save();

        return _config;
    }

    public GuildConfig ChangeWarnPointsTillBan(uint warnPoints)
    {
        _config.WarnPointsTillBan = warnPoints;

        Save();

        return _config;
    }

    public GuildConfig ChangeWarnPointsTillPunishment(uint warnPoints)
    {
        _config.WarnPointsTillPunishment = warnPoints;

        Save();

        return _config;
    }

    public GuildConfig ChangeAnnouncementChannel(ulong id)
    {
        _config.AnnouncementChannelId = id;

        Save();

        return _config;
    }

    public GuildConfig ChangeToSChannel(ulong id)
    {
        _config.ToSChannelId = id;

        Save();

        return _config;
    }

    public GuildConfig ToggleReactionToBotMessages(bool toggle)
    {
        _config.ReactToBotMessages = toggle;

        Save();

        return _config;
    }
}
