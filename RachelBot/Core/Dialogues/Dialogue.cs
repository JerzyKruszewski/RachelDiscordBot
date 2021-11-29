using Discord.Commands;
using Discord.WebSocket;
using RachelBot.Core.Configs;
using RachelBot.Services.Storage;
using RachelBot.Utils;

namespace RachelBot.Core.Dialogues;

public class Dialogue
{
    private static readonly IStorageService _storage = new JsonStorageService();
    private static readonly IDictionary<string, string> _dialogues;
    private static readonly string _filePath = "./Dialogues.json";

    static Dialogue()
    {
        if (_storage.FileExist(_filePath))
        {
            _dialogues = _storage.RestoreObject<Dictionary<string, string>>(_filePath);

            return;
        }

        _dialogues = new Dictionary<string, string>()
        {
            {
                new DialogueCriterion()
                {
                    IsNSFW = false,
                    Id = 1
                }.ToString(),
                "I'm feeling great! Thanks for asking!"
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = false,
                    Id = 2
                }.ToString(),
                "Hello there"
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = false,
                    Id = 3
                }.ToString(),
                "Hello 👋"
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = false,
                    Id = 4
                }.ToString(),
                "👋"
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = false,
                    Id = 5
                }.ToString(),
                "I was wondering how many people will join my server..."
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = true,
                    Id = 1
                }.ToString(),
                "I was wondering what will happen if your parents caught you watching this channel?"
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = true,
                    Id = 2
                }.ToString(),
                "*Umm! Watching NSFW channels we are.* - Yoda"
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = false,
                    StaffOnly = true,
                    Id = 1
                }.ToString(),
                "Oh! Staff member! It's an honor! I hope you are happy with my work here."
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = false,
                    StaffOnly = true,
                    Id = 2
                }.ToString(),
                "Have you tried my raffles?"
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = false,
                    StartHour = 23,
                    EndHour = 5,
                    Id = 1
                }.ToString(),
                "It's night for me... I should be asleep... What am I doing with my life?..."
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = false,
                    StartHour = 6,
                    EndHour = 11,
                    Id = 1
                }.ToString(),
                "Good Morning guys!"
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = false,
                    StartHour = 12,
                    EndHour = 16,
                    Id = 1
                }.ToString(),
                "Time for lunch break!"
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = false,
                    StartHour = 12,
                    EndHour = 17,
                    Id = 1
                }.ToString(),
                "Good Afternoon!"
            },
            {
                new DialogueCriterion()
                {
                    IsNSFW = false,
                    StartHour = 18,
                    EndHour = 22,
                    Id = 2
                }.ToString(),
                "Good Evening!"
            }
        };
        Save();
    }

    private static void Save()
    {
        _storage.StoreObject(_dialogues, _filePath);
    }

    public static string GetRandomResponse(SocketCommandContext context)
    {
        List<string> dialogues = FilterDialogues(context);
        return dialogues[Utility.random.Next(0, dialogues.Count)];
    }

    public static List<string> FilterDialogues(SocketCommandContext context)
    {
        List<string> dialogues = new List<string>();

        foreach (KeyValuePair<string, string> dialogue in _dialogues)
        {
            DialogueCriterion criterion = ParseCriterion(dialogue.Key);

            if (!CheckNSFW(context, criterion))
            {
                continue;
            }
            if (!CheckHour(criterion))
            {
                continue;
            }
            if (!CheckStaff(context, criterion))
            {
                continue;
            }

            dialogues.Add(dialogue.Value);
        }

        return dialogues;
    }

    private static DialogueCriterion ParseCriterion(string key)
    {
        string[] parts = key.Split('|');

        return new DialogueCriterion()
        {
            IsNSFW = bool.Parse(parts[0]),
            StaffOnly = bool.Parse(parts[1]),
            StartHour = int.Parse(parts[2]),
            EndHour = int.Parse(parts[3]),
            Id = int.Parse(parts[4])
        };
    }

    private static bool CheckStaff(SocketCommandContext context, DialogueCriterion criterion)
    {
        if (!criterion.StaffOnly)
        {
            return true;
        }

        SocketGuildUser user = context.User as SocketGuildUser;

        if (user.GuildPermissions.Administrator)
        {
            return true;
        }

        GuildConfig config = new GuildConfigs(context.Guild.Id, _storage).GetGuildConfig();

        foreach (ulong roleId in config.StaffRoleIds)
        {
            if (user.Roles.SingleOrDefault(r => r.Id == roleId) is not null)
            {
                return true;
            }
        }

        return false;
    }

    private static bool CheckHour(DialogueCriterion criterion)
    {
        int hour = DateTime.Now.Hour;
        int end = criterion.EndHour;

        if (criterion.StartHour > end)
        {
            end += 24;
        }

        return (hour >= criterion.StartHour && hour <= end);
    }

    private static bool CheckNSFW(SocketCommandContext context, DialogueCriterion criterion)
    {
        if (!criterion.IsNSFW)
        {
            return true;
        }

        return (context.Channel as SocketTextChannel).IsNsfw;
    }
}
