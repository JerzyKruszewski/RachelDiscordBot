using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.UserAccounts
{
    public class Achievement
    {
        public int Id { init; get; }

        public int Value { get; set; } = 15;

        public string Content { get; set; }
    }
}
