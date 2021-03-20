using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.ModerationAnnouncements
{
    public class Announcement
    {
        public ulong MessageId { init; get; }

        public ulong ChannelId { init; get; }

        public string Content { get; set; }
    }
}
