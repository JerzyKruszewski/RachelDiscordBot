using SharpLink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Modules.Audio.Objects
{
    public class AudioQueue
    {
        public ulong GuildId { get; set; }

        public int PlayingTrackIndex { get; set; }

        public List<AudioQueueItem> Queue { get; set; }
    }
}
