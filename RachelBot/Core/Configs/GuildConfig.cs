using System.Collections.Generic;

namespace RachelBot.Core.Configs
{
    public class GuildConfig
    {
        #region General Configuration
        public ulong GuildId { init; get; }

        public string GuildPrefix { get; set; }

        public string GuildLanguageIso { get; set; } = "en";
        #endregion

        #region Moderation
        public IList<ulong> StaffRoleIds { init; get; } = new List<ulong>();

        public ulong ModeratorChannelId { get; set; } = 0;
        #endregion

        #region User joined
        public ulong InChannelId { get; set; } = 0;

        public string WelcomeMessage { get; set; }
        #endregion

        #region User left
        public ulong OutChannelId { get; set; } = 0;

        public string LeftMessage { get; set; }
        #endregion

        #region Punishments
        public ulong PunishmentRoleId { get; set; } = 0;

        public ulong PunishmentChannelId { get; set; } = 0;

        public bool PointBasedWarns { get; set; } = false;

        public uint WarnDuration { get; set; } = 30;

        public uint WarnCountTillBan { get; set; } = 0;

        public uint WarnCountTillPunishment { get; set; } = 0;

        public uint WarnPointsTillBan { get; set; } = 0;

        public uint WarnPointsTillPunishment { get; set; } = 0;
        #endregion

        #region Others
        public ulong AnnouncementChannelId { get; set; } = 0;

        public ulong ToSChannelId { get; set; } = 0;
        #endregion
    }
}
