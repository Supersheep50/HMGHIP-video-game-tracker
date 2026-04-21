namespace PlayedGames.Services;

public class AuthStateService
{
    public bool          IsAuthenticated { get; private set; }
    public FirebaseUser? CurrentUser     { get; private set; }
    public bool          IsGuest         => IsAuthenticated && CurrentUser == null;

    public event Action? OnChange;

    public void SignInWithUser(FirebaseUser user)
    {
        CurrentUser     = user;
        IsAuthenticated = true;
        OnChange?.Invoke();
    }

    public void SignInAsGuest()
    {
        CurrentUser     = null;
        IsAuthenticated = true;
        OnChange?.Invoke();
    }

    public void SignOut()
    {
        CurrentUser     = null;
        IsAuthenticated = false;
        OnChange?.Invoke();
    }
}
