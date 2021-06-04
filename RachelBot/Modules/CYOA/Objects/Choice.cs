namespace RachelBot.Modules.CYOA.Objects
{
    public class Choice
    {
        public int Id { init; get; }

        public string Content { get; set; }

        public int Weight { get; set; }

        public int PointsToPageWithId { get; set; }
    }
}
