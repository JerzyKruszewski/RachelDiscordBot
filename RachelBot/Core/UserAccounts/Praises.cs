using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.UserAccounts
{
    public class Praises
    {
        public static Praise GetPraise(UserAccount user, uint id)
        {
            return user.Praises.SingleOrDefault(p => p.Id == id);
        }

        public static Praise CreatePraise(ulong giverId, uint id, string reason)
        {
            return new Praise()
            {
                Id = id,
                GiverId = giverId,
                Reason = reason,
                GivenAt = DateTime.Now
            };
        }
    }
}
