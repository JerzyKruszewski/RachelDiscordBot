using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.UserAccounts
{
    public class Praise
    {
        public uint Id { init; get; }

        public ulong GiverId { init; get; }

        public string Reason { get; set; }

        public DateTime GivenAt { init; get; } = DateTime.Now;
    }
}
