namespace RachelBot.Core.Games.TicTacToe;

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

    public static KeyValuePair<int, int>? GetCords(string input)
    {
        if (!ValidateInput(input))
        {
            return null;
        }

        int cords = int.Parse(input);
        int row = (cords - 1) / 3;
        int column = (cords - 1) % 3;

        return new KeyValuePair<int, int>(row, column);
    }

    private static bool ValidateInput(string input)
    {
        return input switch
        {
            "1" or "2" or "3" or "4" or "5" or "6" or "7" or "8" or "9" => true,
            _ => false,
        };
    }
}
