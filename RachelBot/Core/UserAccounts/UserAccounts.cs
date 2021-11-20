using RachelBot.Core.Configs;
using RachelBot.Services.Storage;

namespace RachelBot.Core.UserAccounts;

public class UserAccounts
{
    private readonly IStorageService _storage;
    private readonly string _folderPath;
    private readonly string _filePath;
    private readonly IList<UserAccount> _accounts;

    public UserAccounts(ulong id, IStorageService storage)
    {
        _storage = storage;
        _folderPath = $"./Guilds/{id}";

        _storage.EnsureDirectoryExist(_folderPath);

        _filePath = $"{_folderPath}/UserAccounts.json";

        if (_storage.FileExist(_filePath))
        {
            _accounts = _storage.RestoreObject<List<UserAccount>>(_filePath);
        }
        else
        {
            _accounts = new List<UserAccount>();
            Save();
        }
    }

    private void Save()
    {
        _storage.StoreObject(_accounts, _filePath);
    }

    public IList<UserAccount> GetUserAccounts()
    {
        return _accounts;
    }

    public UserAccount GetUserAccount(ulong id)
    {
        UserAccount account = _accounts.SingleOrDefault(u => u.Id == id);

        if (account is null)
        {
            return CreateAccount(id);
        }

        return account;
    }

    private UserAccount CreateAccount(ulong id)
    {
        UserAccount account = new UserAccount()
        {
            Id = id
        };

        _accounts.Add(account);
        Save();

        return account;
    }

    public void AddXP(UserAccount account, ulong exp)
    {
        account.XP += exp;
        Save();
    }

    public void AddPraise(UserAccount account, ulong giverId, string reason)
    {
        account.Praises.Add(Praises.CreatePraise(giverId, Praises.GetNextId(account.Praises), reason));
        Save();
    }

    public void RemovePraise(UserAccount account, int id)
    {
        Praise praise = account.Praises.SingleOrDefault(p => p.Id == id);

        if (praise is null)
        {
            return;
        }

        account.Praises.Remove(praise);
        Save();
    }

    public Warning AddWarning(UserAccount account, string reason, GuildConfig config)
    {
        Warning warn;

        if (config.PointBasedWarns)
        {
            string[] words = reason.Split(' ');
            int value = int.Parse(reason.Split(' ')[0]);
            reason = words[1];

            for (int i = 2; i < words.Length; i++)
            {
                reason += $" {words[i]}";
            }

            warn = Warnings.CreateWarning(Warnings.GetNextId(account.Warnings), value, reason, config.WarnDuration);
        }
        else
        {
            warn = Warnings.CreateWarning(Warnings.GetNextId(account.Warnings), reason, config.WarnDuration);
        }

        account.Warnings.Add(warn);
        Save();

        return warn;
    }

    public void RemoveWarning(UserAccount account, int warnId)
    {
        account.Warnings.Remove(account.Warnings.Single(w => w.Id == warnId));

        Save();
    }

    public void DeleteExpiredWarnings(UserAccount account)
    {
        try
        {
            for (int i = account.Warnings.Count - 1; i >= 0; i--)
            {
                if (DateTime.Compare(DateTime.Now, account.Warnings[i].ExpireDate) >= 0)
                {
                    account.Warnings.RemoveAt(i);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        Save();
    }

    public static int GetWarningsCount(UserAccount account)
    {
        return account.Warnings.Count;
    }

    public static int GetWarningsPower(UserAccount account)
    {
        int power = 0;

        foreach (Warning warn in account.Warnings)
        {
            power += warn.Value;
        }

        return power;
    }

    public void AddAchievement(UserAccount account, int value, string content)
    {
        Achievement achievement = Achievements.CreateAchievement(Achievements.GetNextId(account.Achievements), value, content);
        account.Achievements.Add(achievement);
        Save();
    }

    public void RemoveAchievement(UserAccount account, int id)
    {
        Achievement achievement = account.Achievements.SingleOrDefault(a => a.Id == id);

        if (achievement is null)
        {
            return;
        }

        account.Achievements.Remove(achievement);
        Save();
    }

    public static int GetAchievementsTotalPoints(UserAccount account)
    {
        int points = 0;

        foreach (Achievement achievement in account.Achievements)
        {
            points += achievement.Value;
        }

        return points;
    }
}
