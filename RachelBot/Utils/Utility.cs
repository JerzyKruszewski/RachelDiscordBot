using Discord;
using Discord.WebSocket;
using RachelBot.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Utils
{
    public class Utility
    {
        public const string Version = "1.2.0";

        public const string DiscordInviteLink = @"https://discord.gg/TjCDEQU";

        public const string GitHubPage = @"https://github.com/JerzyKruszewski/RachelDiscordBot";

        public const string SaraArtistInstagram = @"https://www.instagram.com/aster_atheris/";

        public static ISocketMessageChannel GetMessageChannelById(SocketGuild guild, ulong id)
        {
            return guild.TextChannels.SingleOrDefault(c => c.Id == id);
        }

        public static IRole GetRoleById(SocketGuild guild, ulong id)
        {
            return guild.Roles.SingleOrDefault(r => r.Id == id);
        }

        public static SocketGuildUser GetGuildUserById(SocketGuild guild, ulong id)
        {
            return guild.Users.SingleOrDefault(u => u.Id == id);
        }

        public static string ParseReason(string reason, GuildConfig config)
        {
            if (config.PointBasedWarns)
            {
                string[] words = reason.Split(' ');
                reason = words[1];

                for (int i = 2; i < words.Length; i++)
                {
                    reason += $" {words[i]}";
                }
            }

            return reason;
        }
    }
}
