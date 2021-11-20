using RachelBot.Services.Storage;

namespace RachelBot.Core.LevelingSystem;

public class LevelRoleRewards
{
    private readonly IStorageService _storage;
    private readonly string _folderPath;
    private readonly string _filePath;
    private readonly IList<LevelRoleReward> _roleRewards;

    public LevelRoleRewards(ulong id, IStorageService storage)
    {
        _storage = storage;
        _folderPath = $"./Guilds/{id}";

        _storage.EnsureDirectoryExist(_folderPath);

        _filePath = $"{_folderPath}/RoleRewards.json";

        if (_storage.FileExist(_filePath))
        {
            _roleRewards = _storage.RestoreObject<List<LevelRoleReward>>(_filePath);
        }
        else
        {
            _roleRewards = new List<LevelRoleReward>();
            Save();
        }
    }

    private void Save()
    {
        _storage.StoreObject(_roleRewards, _filePath);
    }

    public LevelRoleReward GetLevelRoleReward(ulong id)
    {
        return _roleRewards.SingleOrDefault(r => r.RoleId == id);
    }

    public IList<LevelRoleReward> GetLevelRoleRewards()
    {
        return _roleRewards;
    }

    public LevelRoleReward CreateLevelRoleReward(ulong id, uint level)
    {
        LevelRoleReward reward = _roleRewards.SingleOrDefault(r => r.RoleId == id);

        if (reward is null)
        {
            reward = new LevelRoleReward()
            {
                RoleId = id,
                RequiredLevel = level
            };

            _roleRewards.Add(reward);
        }
        else
        {
            reward.RequiredLevel = level;
        }

        Save();

        return reward;
    }

    public void RemoveLevelRoleReward(ulong id)
    {
        LevelRoleReward reward = _roleRewards.SingleOrDefault(r => r.RoleId == id);

        if (reward is null)
        {
            return;
        }

        _roleRewards.Remove(reward);

        Save();
    }
}
