namespace RachelBot.Modules.CYOA.Objects;

public class Page
{
    public int Id { init; get; }

    public string DevelopmentCode { get; set; } //Unique identifier containing previous choices

    public string ImageName { get; set; } = "";

    public bool AreChoicesRandom { get; set; } = false;

    public string RandomChoiceContent { get; set; } = "";

    public string Content { get; set; }

    public IList<Choice> Choices { init; get; }
}
