namespace PlayedGames.Modals
{
    public record XpLevel(int Number, string Name, string Badge, int XpRequired);

    public static class XpLevels
    {
        public static readonly IReadOnlyList<XpLevel> All = new List<XpLevel>
        {
            new(1,  "Newbie",         "🎮", 0),
            new(2,  "Button Masher",  "🕹️", 50),
            new(3,  "Casual",         "👾", 150),
            new(4,  "Speedrunner",    "⚡", 350),
            new(5,  "Hardcore",       "🔥", 700),
            new(6,  "Veteran",        "🎖️", 1200),
            new(7,  "Pro",            "🏆", 2000),
            new(8,  "Elite",          "💎", 3000),
            new(9,  "Legendary",      "👑", 4500),
            new(10, "Final Boss",     "🐉", 6500),
        };

        public static XpLevel CurrentLevel(int xp)
        {
            for (int i = All.Count - 1; i >= 0; i--)
                if (xp >= All[i].XpRequired) return All[i];
            return All[0];
        }

        public static XpLevel? NextLevel(int xp)
        {
            var current = CurrentLevel(xp);
            return current.Number == All.Count ? null : All[current.Number];
        }

        // 0..1 progress through the current level (1.0 if maxed out)
        public static double Progress(int xp)
        {
            var current = CurrentLevel(xp);
            var next = NextLevel(xp);
            if (next == null) return 1.0;
            var span = next.XpRequired - current.XpRequired;
            if (span <= 0) return 1.0;
            return Math.Clamp((double)(xp - current.XpRequired) / span, 0, 1);
        }
    }

    public static class XpRewards
    {
        public const int Added    = 5;   // first time game enters library (any status)
        public const int Finished = 25;
        public const int Rating   = 10;
        public const int Review   = 15;
        public const int Liked    = 2;
        public const int Top3     = 10;
    }
}
