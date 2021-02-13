using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Utils
{
    public class Utility
    {
        public static ISocketMessageChannel GetMessageChannelById(SocketGuild guild, ulong id)
        {
            return guild.TextChannels.SingleOrDefault(c => c.Id == id);
        }

        public static IRole GetRoleById(SocketGuild guild, ulong id)
        {
            return guild.Roles.SingleOrDefault(r => r.Id == id);
        }
    }
}
