using System;

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
