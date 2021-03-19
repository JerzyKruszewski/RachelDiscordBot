using RachelBot.Utils;

namespace RachelBot.Core.Games.TicTacToe.AI
{
    public class RandomAI
    {
        public static int MakeMove(Game game, Player player, Player ai)
        {
            int input;

            do
            {
                input = Utility.random.Next(1, 10);
            } while (!game.IsEmpty(input, player, ai));

            return input;
        }
    }
}
