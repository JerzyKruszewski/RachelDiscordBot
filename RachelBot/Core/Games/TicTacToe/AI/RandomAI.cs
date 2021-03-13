using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.Games.TicTacToe.AI
{
    public class RandomAI
    {
        public static Random random = new Random(DateTime.Now.Millisecond);

        public static int MakeMove(Game game, Player player, Player ai)
        {
            int input;

            do
            {
                input = random.Next(1, 10);
            } while (!game.IsEmpty(input, player, ai));

            return input;
        }
    }
}
