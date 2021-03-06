using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.UserAccounts
{
    public class Achievements
    {
        public static Achievement GetAchievement(UserAccount user, int id)
        {
            return user.Achievements.SingleOrDefault(a => a.Id == id);
        }

        public static Achievement CreateAchievement(int id, string content)
        {
            return new Achievement()
            {
                Id = id,
                Content = content
            };
        }

        public static int GetNextId(IList<Achievement> achievements)
        {
            try
            {
                return achievements.Max(w => w.Id) + 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}
