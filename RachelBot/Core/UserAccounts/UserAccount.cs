﻿namespace RachelBot.Core.UserAccounts;

public class UserAccount
{
    public ulong Id { init; get; }

    public ulong XP { get; set; } = 0;

    public uint LevelNumber
    {
        get
        {
            return (uint)Math.Sqrt(XP / 50);
        }
    }

    public bool IsMuted { get; set; } = false;

    public IList<Praise> Praises { init; get; } = new List<Praise>();

    public IList<Warning> Warnings { init; get; } = new List<Warning>();

    public IList<Achievement> Achievements { init; get; } = new List<Achievement>();
}
