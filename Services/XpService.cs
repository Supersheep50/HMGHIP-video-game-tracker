using PlayedGames.Modals;

namespace PlayedGames.Services;

public enum XpAction { Added, Finished, Rating, Review, Liked, Top3 }

public class XpService
{
    private readonly FirebaseService _firebase;
    private readonly AuthStateService _auth;

    private int _xp;
    private HashSet<string> _awarded = new();
    private bool _loaded;
    private string? _loadedForUid;

    public int Xp => _xp;
    public XpLevel CurrentLevel => XpLevels.CurrentLevel(_xp);
    public XpLevel? NextLevel   => XpLevels.NextLevel(_xp);
    public double Progress      => XpLevels.Progress(_xp);
    public bool IsReady         => _loaded;

    public event Action? OnChanged;
    public event Action<XpLevel>? OnLevelUp;
    public event Action<int, XpAction>? OnXpAwarded; // (amount, action)

    public XpService(FirebaseService firebase, AuthStateService auth)
    {
        _firebase = firebase;
        _auth = auth;
        _auth.OnChange += HandleAuthChange;
    }

    private void HandleAuthChange()
    {
        // Reset on sign-out / user switch — next EnsureLoadedAsync re-fetches
        if (_auth.CurrentUser?.Uid != _loadedForUid)
        {
            _xp = 0;
            _awarded = new();
            _loaded = false;
            _loadedForUid = null;
            OnChanged?.Invoke();
        }
    }

    public async Task EnsureLoadedAsync()
    {
        if (_loaded || _auth.CurrentUser == null) return;
        var uid = _auth.CurrentUser.Uid;
        var (xp, awarded) = await _firebase.GetXpAsync(uid);
        _xp = xp;
        _awarded = new HashSet<string>(awarded);
        _loaded = true;
        _loadedForUid = uid;
        OnChanged?.Invoke();
    }

    public async Task AwardAsync(XpAction action, string gameName)
    {
        if (_auth.CurrentUser == null) return;
        await EnsureLoadedAsync();

        var key = $"{action.ToString().ToLowerInvariant()}:{gameName}";
        if (!_awarded.Add(key)) return; // already awarded — dedupe

        var amount = ActionXp(action);
        var prevLevel = CurrentLevel;
        _xp += amount;
        var newLevel = CurrentLevel;

        await _firebase.SaveXpAsync(_auth.CurrentUser.Uid, _xp, _awarded.ToList());

        OnXpAwarded?.Invoke(amount, action);
        OnChanged?.Invoke();
        if (newLevel.Number > prevLevel.Number)
            OnLevelUp?.Invoke(newLevel);
    }

    private static int ActionXp(XpAction a) => a switch
    {
        XpAction.Added    => XpRewards.Added,
        XpAction.Finished => XpRewards.Finished,
        XpAction.Rating   => XpRewards.Rating,
        XpAction.Review   => XpRewards.Review,
        XpAction.Liked    => XpRewards.Liked,
        XpAction.Top3     => XpRewards.Top3,
        _                 => 0,
    };
}
