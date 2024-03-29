﻿using RachelBot.Modules.CYOA.Objects;
using RachelBot.Services.Storage;

namespace RachelBot.Modules.CYOA.Handlers;

public class PlayedStories
{
    private static readonly IStorageService _storage;
    private static readonly string _folderPath;
    private static readonly string _filePath;
    private static readonly IList<PlayedStory> _stories;

    static PlayedStories()
    {
        _storage = new JsonStorageService();

        _folderPath = "./CYOA";

        if (!Directory.Exists(_folderPath))
        {
            Directory.CreateDirectory(_folderPath);
        }

        _filePath = $"{_folderPath}/Stories.json";

        _stories = (_storage.FileExist(_filePath)) ?
                    _storage.RestoreObject<List<PlayedStory>>(_filePath) :
                    new List<PlayedStory>();
    }

    public static void Save()
    {
        _storage.StoreObject(_stories, _filePath);
    }

    public static PlayedStory GetPlayedStory(ulong userId)
    {
        PlayedStory story = _stories.FirstOrDefault(s => s.UserId == userId);

        if (story is null)
        {
            story = new PlayedStory()
            {
                UserId = userId
            };

            _stories.Add(story);

            Save();
        }

        return story;
    }

    public static PlayedStory CreatePlayedStory(ulong userId, string adventureCode, int pageId = 0)
    {
        PlayedStory story = GetPlayedStory(userId);

        if (story is null)
        {
            story = new PlayedStory()
            {
                UserId = userId,
                AdventureCode = adventureCode,
                PageId = pageId
            };

            _stories.Add(story);

            Save();

            return story;
        }

        story.AdventureCode = adventureCode;
        story.PageId = pageId;

        Save();

        return story;
    }

    public static PlayedStory ChangeMC(ulong userId, string mcName, bool isFemale)
    {
        PlayedStory story = GetPlayedStory(userId);

        if (story is null)
        {
            story = new PlayedStory()
            {
                UserId = userId,
                MCName = mcName,
                IsMCFemale = isFemale
            };

            _stories.Add(story);

            Save();

            return story;
        }

        story.MCName = mcName;
        story.IsMCFemale = isFemale;

        Save();

        return story;
    }
}
