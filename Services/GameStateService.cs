namespace PlayedGames.Services;

public class GameStateService
{
    public event Action? OnChanged;
    public void NotifyChanged() => OnChanged?.Invoke();

    public event Action<string, string?, string?, string>? OnOpenGameRequested;
    public void RequestOpenGame(string name, string? artUrl, string? genre, string mode = "actions")
        => OnOpenGameRequested?.Invoke(name, artUrl, genre, mode);
}
