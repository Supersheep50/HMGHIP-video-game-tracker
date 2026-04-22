namespace PlayedGames.Modals
{
    public record FakeFriend(string Name, string AvatarColor, string Initials);

    public class FriendActivity
    {
        public FakeFriend Friend { get; set; } = null!;
        public string GameName { get; set; } = "";
        public string? GameArtUrl { get; set; }
        public string? Genre { get; set; }
        public DateTime When { get; set; }
    }

    public class FriendReview
    {
        public FakeFriend Friend { get; set; } = null!;
        public string GameName { get; set; } = "";
        public string? GameArtUrl { get; set; }
        public string? Genre { get; set; }
        public double Score { get; set; }
        public string ReviewText { get; set; } = "";
        public DateTime When { get; set; }
    }

    public static class FakeFriends
    {
        public static readonly FakeFriend Alex    = new("Alex Chen",    "#f59e0b", "AC");
        public static readonly FakeFriend Jamie   = new("Jamie Park",   "#10b981", "JP");
        public static readonly FakeFriend Sam     = new("Sam Rivera",   "#ec4899", "SR");
        public static readonly FakeFriend Taylor  = new("Taylor Kim",   "#8b5cf6", "TK");
        public static readonly FakeFriend Morgan  = new("Morgan Lee",   "#06b6d4", "ML");
        public static readonly FakeFriend Riley   = new("Riley Patel",  "#ef4444", "RP");

        public static List<FriendActivity> RecentlyFinished() => new()
        {
            new() { Friend = Alex,   GameName = "Elden Ring",                 Genre = "Action RPG",       When = DateTime.Now.AddDays(-2)  },
            new() { Friend = Jamie,  GameName = "Stardew Valley",             Genre = "Simulation",       When = DateTime.Now.AddDays(-5)  },
            new() { Friend = Sam,    GameName = "Hollow Knight",              Genre = "Metroidvania",     When = DateTime.Now.AddDays(-8)  },
            new() { Friend = Taylor, GameName = "Hades",                      Genre = "Roguelike",        When = DateTime.Now.AddDays(-12) },
            new() { Friend = Morgan, GameName = "Disco Elysium",              Genre = "RPG",              When = DateTime.Now.AddDays(-15) },
            new() { Friend = Riley,  GameName = "Baldur's Gate 3",            Genre = "RPG",              When = DateTime.Now.AddDays(-21) },
            new() { Friend = Alex,   GameName = "Cyberpunk 2077",             Genre = "RPG",              When = DateTime.Now.AddDays(-30) },
            new() { Friend = Jamie,  GameName = "The Witcher 3: Wild Hunt",   Genre = "RPG",              When = DateTime.Now.AddDays(-40) },
        };

        public static List<FriendReview> Reviews() => new()
        {
            new() { Friend = Alex,   GameName = "Elden Ring",      Genre = "Action RPG",   Score = 9.5,
                    ReviewText = "Just incredible. Took me 80 hours but I couldn't put it down. The world design is unmatched.",
                    When = DateTime.Now.AddDays(-2) },
            new() { Friend = Jamie,  GameName = "Stardew Valley",  Genre = "Simulation",   Score = 9.0,
                    ReviewText = "Cozy perfection. I lost a weekend to this and have zero regrets.",
                    When = DateTime.Now.AddDays(-5) },
            new() { Friend = Sam,    GameName = "Hollow Knight",   Genre = "Metroidvania", Score = 9.5,
                    ReviewText = "Tough, beautiful, and rewarding. Best metroidvania I've ever played.",
                    When = DateTime.Now.AddDays(-8) },
            new() { Friend = Taylor, GameName = "Hades",           Genre = "Roguelike",    Score = 9.0,
                    ReviewText = "The story unfolds so naturally through gameplay. Genius design from Supergiant.",
                    When = DateTime.Now.AddDays(-12) },
            new() { Friend = Riley,  GameName = "Baldur's Gate 3", Genre = "RPG",          Score = 10.0,
                    ReviewText = "A masterpiece. Every choice matters, every companion feels alive. RPG of the decade.",
                    When = DateTime.Now.AddDays(-21) },
        };
    }
}
