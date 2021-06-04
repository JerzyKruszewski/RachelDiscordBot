namespace RachelBot.Modules.CYOA.Objects
{
    public class PlayedStory
    {
        public ulong UserId { init; get; }

        public string AdventureCode { get; set; } = "";

        public int PageId { get; set; } = 0;

        public string MCName { get; set; } = "John Doe";

        public bool IsMCFemale { get; set; } = false;
    }
}
