using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.Dialogues
{
    public class DialogueCriterion
    {
        public bool IsNSFW { get; set; } = false;

        public bool StaffOnly { get; set; } = false;

        public int StartHour { get; set; } = 0;

        public int EndHour { get; set; } = 24;

        public int Id { get; set; } = 0;

        public override string ToString()
        {
            return $"{IsNSFW}|{StaffOnly}|{StartHour}|{EndHour}|{Id}";
        }
    }
}
