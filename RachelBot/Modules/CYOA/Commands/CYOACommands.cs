using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using RachelBot.Modules.CYOA.Handlers;
using RachelBot.Modules.CYOA.Objects;
using RachelBot.Preconditions;

namespace RachelBot.Modules.CYOA.Commands;

[Group("CYOA")]
public class CYOACommands : InteractiveBase<SocketCommandContext>
{
    [Command("List")]
    [RequireMotherServerMember]
    public async Task GetAvailableStories()
    {
        IList<Adventure> adventures = Adventures.GetAdventures();
        StringBuilder message = new StringBuilder();

        foreach (Adventure adventure in adventures)
        {
            message.Append($"- {adventure.Code} (Language: {adventure.Language} Minimal age: {adventure.MinimalAge})\n");

            if (message.Length >= 1950)
            {
                break;
            }
        }

        Embed embed = new EmbedBuilder()
        {
            Title = $"Available Stories ({adventures.Count} stories)",
            Description = message.ToString(),
            Color = new Color(1, 69, 44)
        }.Build();

        await Context.Channel.SendMessageAsync("", embed: embed);
    }

    [Command("Profile")]
    [RequireMotherServerMember]
    public async Task CheckProfile()
    {
        PlayedStory story = PlayedStories.GetPlayedStory(Context.User.Id);

        Embed embed = new EmbedBuilder()
        {
            Title = $"{Context.User.Username} CYOA Profile",
            Description = $"Current adventure: {story.AdventureCode} ({story.PageId})\n\nMain Character: {story.MCName} ({(story.IsMCFemale ? "F" : "M")})",
            Color = new Color(1, 69, 44)
        }.Build();

        await Context.Channel.SendMessageAsync("", embed: embed);
    }

    [Command("Change Profile")]
    [RequireMotherServerMember]
    public async Task ChangeProfile(bool isFemale, [Remainder]string mcName)
    {
        _ = PlayedStories.ChangeMC(Context.User.Id, mcName, isFemale);

        await CheckProfile();
    }

    [Command("Choose Adventure")]
    [RequireMotherServerMember]
    public async Task ChooseAdventure([Remainder]string code)
    {
        Adventure adventure = Adventures.GetAdventure(code);
        _ = PlayedStories.CreatePlayedStory(Context.User.Id, code, adventure.FirstPageId);

        await CheckProfile();
    }

    [Command("Play")]
    [RequireMotherServerMember]
    public async Task Play()
    {
        PlayedStory story = PlayedStories.GetPlayedStory(Context.User.Id);
        Adventure adventure = Adventures.GetAdventure(story.AdventureCode);
        Page page = Pages.GetPage(adventure, story.PageId);
        StringBuilder message = new StringBuilder($"{Utils.PerParsePageContent(page.Content, adventure, story)}\n\n");

        if (page.AreChoicesRandom)
        {
            message.Append($"*1 - {page.RandomChoiceContent}*");
        }
        else
        {
            foreach (Choice choice in page.Choices)
            {
                message.Append($"*{choice.Id} - {choice.Content}*\n");
            }
        }

        Embed embed = new EmbedBuilder()
        {
            Title = $"{adventure.Name}",
            Description = message.ToString(),
            ImageUrl = page.ImageName,
            Color = new Color(1, 69, 44)
        }.Build();

        await Context.Channel.SendMessageAsync("", embed: embed);
    }

    [Command("Make Choice")]
    [RequireMotherServerMember]
    public async Task MakeChoice(int id)
    {
        PlayedStory story = PlayedStories.GetPlayedStory(Context.User.Id);
        Adventure adventure = Adventures.GetAdventure(story.AdventureCode);
        Page page = Pages.GetPage(adventure, story.PageId);

        story.PageId = (page.AreChoicesRandom) ?
                       Choices.GetRandomChoice(page).PointsToPageWithId :
                       (page.Choices.SingleOrDefault(c => c.Id == id)?.PointsToPageWithId ?? story.PageId);

        PlayedStories.Save();

        await Play();
    }
}
