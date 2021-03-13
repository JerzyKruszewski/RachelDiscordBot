using System;
using System.Collections.Generic;

namespace RachelBot.Core.Games.TicTacToe
{
    public class Player
    {
        private readonly ulong _id;

        private readonly string _character;

        public Player(ulong id, string character)
        {
            _id = id;
            _character = character;
        }

        public ulong Id
        {
            get
            {
                return _id;
            }
        }

        public string Character
        {
            get
            {
                return _character;
            }
        }

        public KeyValuePair<int, int>? GetCords(string input)
        {
            if (!ValidateInput(input))
            {
                return null;
            }

            int cords = Int32.Parse(input);
            int row = (cords - 1) / 3;
            int column = (cords - 1) % 3;

            return new KeyValuePair<int, int>(row, column);
        }

        private bool ValidateInput(string input)
        {
            switch (input)
            {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    return true;
                default:
                    return false;
            }
        }
    }
}
