using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.UserAccounts
{
    public class Warning
    {
        public int Id { init; get; }

        public int Value { init; get; } = 20;
        
        public string Reason { get; set; }

        public DateTime GivenAt { init; get; } = DateTime.Now;

        public DateTime ExpireDate { get; set; }
    }
}
