using System.Collections.Generic;

namespace RachelBot.Core.Games.TicTacToe.AI
{
    public class HardCodedAI
    {
        public static int MakeMove(Game game, Player player, Player ai)
        {
            int input;
            List<int> empty = game.GetAllEmptyElements(player, ai);

            #region Win
            if (((game.Board[0, 1] == ai.Character && game.Board[0, 2] == ai.Character) ||
                 (game.Board[1, 0] == ai.Character && game.Board[2, 0] == ai.Character) ||
                 (game.Board[2, 2] == ai.Character && game.Board[1, 1] == ai.Character)) && empty.Contains(1))
            {
                return 1;
            }

            if (((game.Board[0, 0] == ai.Character && game.Board[0, 2] == ai.Character) ||
                 (game.Board[1, 1] == ai.Character && game.Board[2, 1] == ai.Character)) && empty.Contains(2))
            {
                return 2;
            }

            if (((game.Board[0, 0] == ai.Character && game.Board[0, 1] == ai.Character) ||
                 (game.Board[1, 2] == ai.Character && game.Board[2, 2] == ai.Character) ||
                 (game.Board[2, 0] == ai.Character && game.Board[1, 1] == ai.Character)) && empty.Contains(3))
            {
                return 3;
            }

            if (((game.Board[1, 1] == ai.Character && game.Board[1, 2] == ai.Character) ||
                 (game.Board[0, 0] == ai.Character && game.Board[2, 0] == ai.Character)) && empty.Contains(4))
            {
                return 4;
            }

            if (((game.Board[0, 0] == ai.Character && game.Board[2, 2] == ai.Character) ||
                 (game.Board[1, 0] == ai.Character && game.Board[1, 2] == ai.Character) ||
                 (game.Board[2, 0] == ai.Character && game.Board[0, 2] == ai.Character) ||
                 (game.Board[2, 1] == ai.Character && game.Board[0, 1] == ai.Character)) && empty.Contains(5))
            {
                return 5;
            }

            if (((game.Board[1, 0] == ai.Character && game.Board[1, 1] == ai.Character) ||
                 (game.Board[0, 2] == ai.Character && game.Board[2, 2] == ai.Character)) && empty.Contains(6))
            {
                return 6;
            }

            if (((game.Board[0, 0] == ai.Character && game.Board[1, 0] == ai.Character) ||
                 (game.Board[2, 1] == ai.Character && game.Board[2, 2] == ai.Character) ||
                 (game.Board[0, 2] == ai.Character && game.Board[1, 1] == ai.Character)) && empty.Contains(7))
            {
                return 7;
            }

            if (((game.Board[0, 1] == ai.Character && game.Board[1, 1] == ai.Character) ||
                 (game.Board[2, 0] == ai.Character && game.Board[2, 2] == ai.Character)) && empty.Contains(8))
            {
                return 8;
            }

            if (((game.Board[0, 0] == ai.Character && game.Board[1, 1] == ai.Character) ||
                 (game.Board[0, 2] == ai.Character && game.Board[1, 2] == ai.Character) ||
                 (game.Board[2, 0] == ai.Character && game.Board[2, 1] == ai.Character)) && empty.Contains(9))
            {
                return 9;
            }
            #endregion

            #region Block
            if (((game.Board[0, 1] == player.Character && game.Board[0, 2] == player.Character) ||
                 (game.Board[1, 0] == player.Character && game.Board[2, 0] == player.Character) ||
                 (game.Board[2, 2] == player.Character && game.Board[1, 1] == player.Character)) && empty.Contains(1))
            {
                return 1;
            }

            if (((game.Board[0, 0] == player.Character && game.Board[0, 2] == player.Character) ||
                 (game.Board[1, 1] == player.Character && game.Board[2, 1] == player.Character)) && empty.Contains(2))
            {
                return 2;
            }

            if (((game.Board[0, 0] == player.Character && game.Board[0, 1] == player.Character) ||
                 (game.Board[1, 2] == player.Character && game.Board[2, 2] == player.Character) ||
                 (game.Board[2, 0] == player.Character && game.Board[1, 1] == player.Character)) && empty.Contains(3))
            {
                return 3;
            }

            if (((game.Board[1, 1] == player.Character && game.Board[1, 2] == player.Character) ||
                 (game.Board[0, 0] == player.Character && game.Board[2, 0] == player.Character)) && empty.Contains(4))
            {
                return 4;
            }

            if (((game.Board[0, 0] == player.Character && game.Board[2, 2] == player.Character) ||
                 (game.Board[1, 0] == player.Character && game.Board[1, 2] == player.Character) ||
                 (game.Board[2, 0] == player.Character && game.Board[0, 2] == player.Character) ||
                 (game.Board[2, 1] == player.Character && game.Board[0, 1] == player.Character)) && empty.Contains(5))
            {
                return 5;
            }

            if (((game.Board[1, 0] == player.Character && game.Board[1, 1] == player.Character) ||
                 (game.Board[0, 2] == player.Character && game.Board[2, 2] == player.Character)) && empty.Contains(6))
            {
                return 6;
            }

            if (((game.Board[0, 0] == player.Character && game.Board[1, 0] == player.Character) ||
                 (game.Board[2, 1] == player.Character && game.Board[2, 2] == player.Character) ||
                 (game.Board[0, 2] == player.Character && game.Board[1, 1] == player.Character)) && empty.Contains(7))
            {
                return 7;
            }

            if (((game.Board[0, 1] == player.Character && game.Board[1, 1] == player.Character) ||
                 (game.Board[2, 0] == player.Character && game.Board[2, 2] == player.Character)) && empty.Contains(8))
            {
                return 8;
            }

            if (((game.Board[0, 0] == player.Character && game.Board[1, 1] == player.Character) ||
                 (game.Board[0, 2] == player.Character && game.Board[1, 2] == player.Character) ||
                 (game.Board[2, 0] == player.Character && game.Board[2, 1] == player.Character)) && empty.Contains(9))
            {
                return 9;
            }
            #endregion

            //Random move
            input = empty[RandomAI.random.Next(0, empty.Count)];

            return input;
        }
    }
}
