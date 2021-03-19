using System;
using System.Collections.Generic;

namespace RachelBot.Core.Games.TicTacToe
{
    public class Game
    {
        private readonly string[,] _table;

        public Game()
        {
            _table = new string[3, 3];

            _table[0, 0] = "1️⃣";
            _table[0, 1] = "2️⃣";
            _table[0, 2] = "3️⃣";
            _table[1, 0] = "4️⃣";
            _table[1, 1] = "5️⃣";
            _table[1, 2] = "6️⃣";
            _table[2, 0] = "7️⃣";
            _table[2, 1] = "8️⃣";
            _table[2, 2] = "9️⃣";
        }

        public bool FirstPlayerTurn { get; set; } = true;

        public string[,] Board
        {
            get
            {
                return _table;
            }
        }

        public bool CheckScore()
        {
            return (CheckRows() || CheckColumns() || CheckDiagnals());
        }

        private bool CheckRows()
        {
            return ((_table[0, 0] == _table[0, 1] && _table[0, 1] == _table[0, 2]) ||
                    (_table[1, 0] == _table[1, 1] && _table[1, 1] == _table[1, 2]) ||
                    (_table[2, 0] == _table[2, 1] && _table[2, 1] == _table[2, 2]));
        }

        private bool CheckColumns()
        {
            return ((_table[0, 0] == _table[1, 0] && _table[1, 0] == _table[2, 0]) ||
                    (_table[0, 1] == _table[1, 1] && _table[1, 1] == _table[2, 1]) ||
                    (_table[0, 2] == _table[1, 2] && _table[1, 2] == _table[2, 2]));
        }

        private bool CheckDiagnals()
        {
            return ((_table[0, 0] == _table[1, 1] && _table[1, 1] == _table[2, 2]) ||
                    (_table[0, 2] == _table[1, 1] && _table[1, 1] == _table[2, 0]));
        }

        public Player CheckWinner(Player player, Player ai)
        {
            if (!CheckScore())
            {
                return null;
            }

            return (FirstPlayerTurn) ? player : ai;
        }

        public string ShowTable()
        {
            return $@"
 {_table[0, 0]} | {_table[0, 1]} | {_table[0, 2]}
==========
 {_table[1, 0]} | {_table[1, 1]} | {_table[1, 2]}
==========
 {_table[2, 0]} | {_table[2, 1]} | {_table[2, 2]}
";
        }

        public void ChangeTableElement(int row, int column, string changeTo)
        {
            if (_table[row, column] == "❌" || _table[row, column] == "⭕")
            {
                throw new Exception("Wrong input!");
            }

            _table[row, column] = changeTo;
        }

        public void ChangePlayer()
        {
            FirstPlayerTurn = !FirstPlayerTurn;
        }

        public bool IsEmpty(int place, Player firstPlayer, Player secondPlayer)
        {
            string placeChar = _table[(place - 1) / 3, (place - 1) % 3];

            if (placeChar != firstPlayer.Character && placeChar != secondPlayer.Character)
            {
                return true;
            }

            return false;
        }

        public List<int> GetAllEmptyElements(Player firstPlayer, Player secondPlayer)
        {
            List<int> empty = new List<int>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (_table[i, j] != firstPlayer.Character && _table[i, j] != secondPlayer.Character)
                    {
                        empty.Add((i * 3) + j + 1);
                    }
                }
            }

            return empty;
        }
    }
}
