namespace PlayedGames.Modals
{
    public class Games
    {
        public string? Name { get; set; }
        public string? System { get; set; }
        public string? Genre { get; set; }
        public string? Developer { get; set; }
        public double HoursPlayed { get; set; }
        public double Review { get; set; }
        public double HowLongToBeat { get; set; }
        public string? GameArtUrl { get; set; }
        public int Year { get; set; } = DateTime.Now.Year;
        public string? ReviewText { get; set; }
    }
}
