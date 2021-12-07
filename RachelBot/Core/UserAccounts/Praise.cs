namespace RachelBot.Core.UserAccounts;

public class Praise
{
    public int Id { init; get; }

    public ulong GiverId { init; get; }

    public string Reason { get; set; }

    public DateTime GivenAt { init; get; } = DateTime.Now;
}
