using RachelBot.Services.Storage;

namespace RachelBot.Core.ModerationAnnouncements;

public class Announcements
{
    private readonly IStorageService _storage;
    private readonly string _folderPath;
    private readonly string _filePath;
    private readonly IList<Announcement> _announcements;

    public Announcements(ulong id, IStorageService storage)
    {
        _storage = storage;
        _folderPath = $"./Guilds/{id}";

        _storage.EnsureDirectoryExist(_folderPath);

        _filePath = $"{_folderPath}/Announcements.json";

        if (_storage.FileExist(_filePath))
        {
            _announcements = _storage.RestoreObject<List<Announcement>>(_filePath);
        }
        else
        {
            _announcements = new List<Announcement>();
            Save();
        }
    }

    private void Save()
    {
        _storage.StoreObject(_announcements, _filePath);
    }

    public Announcement CreateAnnouncement(ulong messageId, ulong channelId, string content)
    {
        Announcement announcement = new Announcement()
        {
            MessageId = messageId,
            ChannelId = channelId,
            Content = content
        };

        _announcements.Add(announcement);
        Save();

        return announcement;
    }

    public Announcement UpdateAnnouncement(ulong messageId, string newContent)
    {
        Announcement announcement = GetAnnouncement(messageId);

        if (announcement is null)
        {
            return null;
        }

        announcement.Content = newContent;
        Save();

        return announcement;
    }

    private Announcement GetAnnouncement(ulong messageId)
    {
        return _announcements.SingleOrDefault(a => a.MessageId == messageId);
    }
}
