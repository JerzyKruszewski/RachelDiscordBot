using System;
using System.Collections.Generic;
using System.Linq;

namespace RachelBot.Core.UserAccounts
{
    public class Praises
    {
        public static Praise GetPraise(UserAccount user, int id)
        {
            return user.Praises.SingleOrDefault(p => p.Id == id);
        }

        public static Praise CreatePraise(ulong giverId, int id, string reason)
        {
            return new Praise()
            {
                Id = id,
                GiverId = giverId,
                Reason = reason,
                GivenAt = DateTime.Now
            };
        }

        public static int GetNextId(IList<Praise> praises)
        {
            try
            {
                return praises.Max(w => w.Id) + 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}
