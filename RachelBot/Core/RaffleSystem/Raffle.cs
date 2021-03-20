using System.Collections.Generic;

namespace RachelBot.Core.RaffleSystem
{
    public class Raffle
    {
        public int Id { init; get; }

        public string Reward { get; set; }

        public bool UsersCanEnter { get; set; }

        public IList<ulong> EnteredUsers { init; get; }

        public IDictionary<ulong, int> Tickets { init; get; }

        public ulong Winner { get; set; } = 0;

        public bool IsEnded
        {
            get
            {
                return Winner != 0;
            }
        }
    }
}
