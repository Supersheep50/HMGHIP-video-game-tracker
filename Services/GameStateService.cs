namespace PlayedGames.Services;

public class GameStateService
{
    public event Action? OnChanged;
    public void NotifyChanged() => OnChanged?.Invoke();

    public event Action<string, string?, string?>? OnOpenGameRequested;
    public void RequestOpenGame(string name, string? artUrl, string? genre)
        => OnOpenGameRequested?.Invoke(name, artUrl, genre);
}
