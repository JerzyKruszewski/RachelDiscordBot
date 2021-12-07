using RachelBot.Services.Storage;
using RachelBot.Utils;

namespace RachelBot.Core.RaffleSystem;

public class Raffles
{
    private readonly IStorageService _storage;
    private readonly string _folderPath;
    private readonly string _filePath;
    private readonly IList<Raffle> _raffles;

    public Raffles(ulong id, IStorageService storage)
    {
        _storage = storage;
        _folderPath = $"./Guilds/{id}";

        _storage.EnsureDirectoryExist(_folderPath);

        _filePath = $"{_folderPath}/Raffles.json";

        if (_storage.FileExist(_filePath))
        {
            _raffles = _storage.RestoreObject<List<Raffle>>(_filePath);
            return;
        }

        _raffles = new List<Raffle>();
        Save();
    }

    private void Save()
    {
        _storage.StoreObject(_raffles, _filePath);
    }

    public Raffle CreateRaffle(string reward, bool usersCanEnter)
    {
        Raffle raffle = new Raffle()
        {
            Id = GetNextId(),
            UsersCanEnter = usersCanEnter,
            Reward = reward,
            EnteredUsers = new List<ulong>(),
            Tickets = new Dictionary<ulong, int>()
        };

        _raffles.Add(raffle);
        Save();

        return raffle;
    }

    public Raffle GetRaffle(int id)
    {
        return _raffles.SingleOrDefault(r => r.Id == id);
    }

    public IList<Raffle> GetRaffles()
    {
        return _raffles;
    }

    public bool AddTickets(Raffle raffle, ulong userId, int tickets, bool byUser)
    {
        if (byUser && !raffle.UsersCanEnter)
        {
            return false;
        }

        if (byUser)
        {
            if (raffle.EnteredUsers.Contains(userId))
            {
                return false;
            }

            raffle.EnteredUsers.Add(userId);
        }

        if (raffle.Tickets.ContainsKey(userId))
        {
            raffle.Tickets[userId] += tickets;

            Save();

            return true;
        }

        raffle.Tickets.Add(userId, tickets);
            
        Save();

        return true;
    }

    public Raffle Roll(Raffle raffle)
    {
        raffle.Winner = GetWinner(raffle);

        Save();

        return raffle;
    }

    private ulong GetWinner(Raffle raffle)
    {
        List<double> cumulativeDistribution = new List<double>()
        {
            0.0
        };

        double sumOfTickets = raffle.Tickets.Values.Sum();
        double randomNumber = Utility.random.NextDouble();

        foreach (int value in raffle.Tickets.Values)
        {
            cumulativeDistribution.Add(cumulativeDistribution[^1] + (value / sumOfTickets));
        }

        for (int i = 0; i < cumulativeDistribution.Count - 1; i++)
        {
            if (randomNumber > cumulativeDistribution[i] && randomNumber <= cumulativeDistribution[i + 1])
            {
                return raffle.Tickets.ToList()[i].Key;
            }
        }

        return raffle.Tickets.Last().Key;
    }

    private int GetNextId()
    {
        try
        {
            return _raffles.Max(w => w.Id) + 1;
        }
        catch (Exception)
        {
            return 1;
        }
    }
}
