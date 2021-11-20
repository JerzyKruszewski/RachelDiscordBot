namespace RachelBot.Modules.CYOA.Objects;

public class Adventure
{
    public int Id { init; get; }

    public string Author { init; get; } //Add as separate object

    public string Name { init; get; }

    public string Language { init; get; }

    public string Code
    {
        get
        {
            return $"{Id}_{Name.Replace(' ', '_')}_{Author}";
        }
    }

    public string MCName { get; set; } = "";

    public bool? IsMCFemale { get; set; } = null;

    public int MinimalAge { get; set; }

    public int FirstPageId { get; set; }

    public IList<Page> Pages { init; get; }
}
