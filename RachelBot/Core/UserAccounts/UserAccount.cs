using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.UserAccounts
{
    public class UserAccount
    {
        public ulong Id { init; get; }

        public ulong XP { get; set; } = 0;

        public uint LevelNumber
        {
            get
            {
                return (uint)Math.Sqrt(XP / 50);
            }
        }

        public bool IsMuted { get; set; } = false;

        public IList<Praise> Praises { init; get; } = new List<Praise>();

        public IList<Warning> Warnings { init; get; } = new List<Warning>();

        public IList<string> Archievements { init; get; } = new List<string>();
    }
}
