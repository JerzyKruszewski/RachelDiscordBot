using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.UserAccounts
{
    public class Warnings
    {
        public static Warning GetWarning(UserAccount user, uint id)
        {
            return user.Warnings.SingleOrDefault(w => w.Id == id);
        }

        public static Warning CreateWarning(int id, string reason, uint daysToExpire)
        {
            return new Warning()
            {
                Id = id,
                Reason = reason,
                ExpireDate = DateTime.Now + TimeSpan.FromDays(daysToExpire)
            };
        }

        public static Warning CreateWarning(int id, int value, string reason, uint daysToExpire)
        {
            return new Warning()
            {
                Id = id,
                Value = value,
                Reason = reason,
                ExpireDate = DateTime.Now + TimeSpan.FromDays(daysToExpire)
            };
        }

        public static int GetNextId(IList<Warning> warnings)
        {
            try
            {
                return warnings.Max(w => w.Id);
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}
