using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace PlayedGames.Modals;

// Single source of truth for genre gradients/icons and rating display,
// shared by every page and the game-detail popup.
public static class GameVisuals
{
    public static string Gradient(string? genre) => genre switch
    {
        "Action RPG"       => "linear-gradient(145deg, #1e0a3c 0%, #6b21a8 100%)",
        "RPG"              => "linear-gradient(145deg, #0d1b4b 0%, #1d4ed8 100%)",
        "Action-Adventure" => "linear-gradient(145deg, #1c0505 0%, #991b1b 100%)",
        "Adventure"        => "linear-gradient(145deg, #1c0505 0%, #991b1b 100%)",
        "JRPG"             => "linear-gradient(145deg, #3b0000 0%, #dc2626 100%)",
        "Roguelike"        => "linear-gradient(145deg, #0a0a1a 0%, #4338ca 100%)",
        "Indie"            => "linear-gradient(145deg, #0a0a1a 0%, #4338ca 100%)",
        "Metroidvania"     => "linear-gradient(145deg, #000d10 0%, #0e7490 100%)",
        "Puzzle"           => "linear-gradient(145deg, #000d10 0%, #0e7490 100%)",
        "Simulation"       => "linear-gradient(145deg, #0a1a0a 0%, #15803d 100%)",
        "Strategy"         => "linear-gradient(145deg, #0a1a0a 0%, #15803d 100%)",
        "MMORPG"           => "linear-gradient(145deg, #1a1000 0%, #b45309 100%)",
        "Massively Multiplayer" => "linear-gradient(145deg, #1a1000 0%, #b45309 100%)",
        "Action"           => "linear-gradient(145deg, #1a0a00 0%, #c2410c 100%)",
        "Shooter"          => "linear-gradient(145deg, #1a0a00 0%, #c2410c 100%)",
        "Platformer"       => "linear-gradient(145deg, #0a1a2a 0%, #0369a1 100%)",
        "Racing"           => "linear-gradient(145deg, #1a0a00 0%, #d97706 100%)",
        "Sports"           => "linear-gradient(145deg, #1a0a00 0%, #d97706 100%)",
        _                  => "linear-gradient(145deg, #1a1a2e 0%, #374151 100%)",
    };

    public static string Icon(string? genre) => genre switch
    {
        "Action RPG"       => Icons.Material.Filled.AutoFixHigh,
        "RPG"              => Icons.Material.Filled.AutoAwesome,
        "Action-Adventure" => Icons.Material.Filled.Explore,
        "Adventure"        => Icons.Material.Filled.Explore,
        "JRPG"             => Icons.Material.Filled.Stars,
        "Roguelike"        => Icons.Material.Filled.Loop,
        "Indie"            => Icons.Material.Filled.Lightbulb,
        "Metroidvania"     => Icons.Material.Filled.Terrain,
        "Puzzle"           => Icons.Material.Filled.Extension,
        "Simulation"       => Icons.Material.Filled.Yard,
        "Strategy"         => Icons.Material.Filled.Psychology,
        "MMORPG"           => Icons.Material.Filled.Groups,
        "Massively Multiplayer" => Icons.Material.Filled.Groups,
        "Action"           => Icons.Material.Filled.FlashOn,
        "Shooter"          => Icons.Material.Filled.GpsFixed,
        "Platformer"       => Icons.Material.Filled.DirectionsRun,
        "Racing"           => Icons.Material.Filled.Speed,
        "Sports"           => Icons.Material.Filled.SportsSoccer,
        _                  => Icons.Material.Filled.SportsEsports,
    };

    public static string RatingClass(double score) => score switch
    {
        >= 9 => "rating-excellent",
        >= 7 => "rating-good",
        >= 5 => "rating-average",
        _    => "rating-poor",
    };

    // Score is /10; stars render /5 (half-star precision)
    public static MarkupString Stars(double score)
    {
        double stars = score / 2.0;
        int full  = (int)Math.Floor(stars);
        bool half = (stars - full) >= 0.5;
        int empty = 5 - full - (half ? 1 : 0);
        const string F = "<svg class='star star-full'  viewBox='0 0 24 24'><path d='M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z'/></svg>";
        const string H = "<svg class='star star-half'  viewBox='0 0 24 24'><path d='M22 9.24l-7.19-.62L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21 12 17.27 18.18 21l-1.63-7.03L22 9.24zM12 15.4V6.1l1.71 4.04 4.38.38-3.32 2.88 1 4.28L12 15.4z'/></svg>";
        const string E = "<svg class='star star-empty' viewBox='0 0 24 24'><path d='M22 9.24l-7.19-.62L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21 12 17.27 18.18 21l-1.63-7.03L22 9.24zM12 15.4l-3.76 2.27 1-4.28-3.32-2.88 4.38-.38L12 6.1l1.71 4.04 4.38.38-3.32 2.88 1 4.28L12 15.4z'/></svg>";
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < full;  i++) sb.Append(F);
        if (half) sb.Append(H);
        for (int i = 0; i < empty; i++) sb.Append(E);
        return new MarkupString(sb.ToString());
    }
}
