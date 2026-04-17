namespace PlayedGames.Services;

/// <summary>
/// Tracks whether the user has authenticated (or chosen guest) for this session.
/// Replaced by real Firebase Auth when that is wired up.
/// </summary>
public class AuthStateService
{
    public bool IsAuthenticated { get; private set; }

    public void SignInAsGuest()  => IsAuthenticated = true;
    public void SignIn()         => IsAuthenticated = true;
    public void SignOut()        => IsAuthenticated = false;
}
