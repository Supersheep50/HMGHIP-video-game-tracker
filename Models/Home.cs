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
        public bool Liked { get; set; }
        // Diary timestamps — null on entries saved before these existed
        public DateTime? FinishedDate { get; set; }
        public DateTime? ReviewDate { get; set; }

        // Best-guess moment for feed ordering; legacy entries fall back to Jan 1 of their year
        public DateTime DiaryDate => ReviewDate ?? FinishedDate ?? new DateTime(Math.Clamp(Year, 2000, 9999), 1, 1);
    }
}
