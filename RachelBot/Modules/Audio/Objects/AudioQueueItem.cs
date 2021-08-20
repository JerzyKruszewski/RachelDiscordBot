﻿using SharpLink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Modules.Audio.Objects
{
    public class AudioQueueItem
    {
        public ulong RequestedInChannelId { get; set; }

        public LavalinkTrack Track { get; set; }
    }
}
