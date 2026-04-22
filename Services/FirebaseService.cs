using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;
using PlayedGames.Modals;

namespace PlayedGames.Services
{
    public class FirebaseService
    {
        private readonly IJSRuntime _js;
        private readonly HttpClient _http;

        public FirebaseService(IJSRuntime js, HttpClient http)
        {
            _js = js;
            _http = http;
        }

        // ── Auth ──────────────────────────────────────────────────────────────

        public async Task<FirebaseUser?> SignInWithGoogleAsync()
        {
            try
            {
                var result = await _js.InvokeAsync<JsonElement>("firebaseSignInGoogle");
                return ParseUser(result);
            }
            catch { return null; }
        }

        public async Task SignOutAsync()
        {
            try { await _js.InvokeVoidAsync("firebaseSignOut"); } catch { }
        }

        public async Task<FirebaseUser?> GetCurrentUserAsync()
        {
            try
            {
                var result = await _js.InvokeAsync<JsonElement>("firebaseGetCurrentUser");
                if (result.ValueKind == JsonValueKind.Null) return null;
                return ParseUser(result);
            }
            catch { return null; }
        }

        // ── Firestore ─────────────────────────────────────────────────────────

        public async Task SaveGameStatusAsync(string userId, string gameName, string status)
        {
            try
            {
                await _js.InvokeVoidAsync("firestoreSaveGameStatus", userId, gameName, status);
            }
            catch { }
        }

        public async Task<Dictionary<string, string>> GetGameStatusesAsync(string userId)
        {
            try
            {
                var result = await _js.InvokeAsync<JsonElement>("firestoreGetGameStatuses", userId);
                var dict = new Dictionary<string, string>();
                foreach (var prop in result.EnumerateObject())
                    dict[prop.Name] = prop.Value.GetString() ?? "";
                return dict;
            }
            catch { return new Dictionary<string, string>(); }
        }

        public async Task SaveGamesAsync(string userId, List<Games> games)
        {
            try { await _js.InvokeVoidAsync("firestoreSaveGames", userId, games); } catch { }
        }

        public async Task<List<Games>> GetGamesAsync(string userId)
        {
            try
            {
                var result = await _js.InvokeAsync<JsonElement>("firestoreGetGames", userId);
                return result.EnumerateArray().Select(el => new Games
                {
                    Name          = el.TryGetProperty("Name",          out var n)  ? n.GetString()  : null,
                    Genre         = el.TryGetProperty("Genre",         out var g)  ? g.GetString()  : null,
                    System        = el.TryGetProperty("System",        out var s)  ? s.GetString()  : null,
                    Developer     = el.TryGetProperty("Developer",     out var d)  ? d.GetString()  : null,
                    HoursPlayed   = el.TryGetProperty("HoursPlayed",   out var h)  ? h.GetDouble()  : 0,
                    Review        = el.TryGetProperty("Review",        out var r)  ? r.GetDouble()  : 0,
                    HowLongToBeat = el.TryGetProperty("HowLongToBeat", out var lb) ? lb.GetDouble() : 0,
                    GameArtUrl    = el.TryGetProperty("GameArtUrl",    out var a)  ? a.GetString()  : null,
                    Year          = el.TryGetProperty("Year",          out var y)  ? y.GetInt32()   : DateTime.Now.Year,
                }).ToList();
            }
            catch { return new List<Games>(); }
        }

        public async Task SaveTop3Async(string userId, List<string?> gameNames)
        {
            try
            {
                await _js.InvokeVoidAsync("firestoreSaveTop3", userId, gameNames);
            }
            catch { }
        }

        public async Task<List<string?>> GetTop3Async(string userId)
        {
            try
            {
                var result = await _js.InvokeAsync<JsonElement>("firestoreGetTop3", userId);
                return result.EnumerateArray()
                    .Select(e => e.ValueKind == JsonValueKind.Null ? null : e.GetString())
                    .ToList();
            }
            catch { return new List<string?> { null, null, null }; }
        }

        // ── Profile ───────────────────────────────────────────────────────────

        public async Task SaveProfileAsync(string userId, string username)
        {
            try { await _js.InvokeVoidAsync("firestoreSaveProfile", userId, username); } catch { }
        }

        public async Task<UserProfile?> GetProfileAsync(string userId)
        {
            try
            {
                var result = await _js.InvokeAsync<JsonElement>("firestoreGetProfile", userId);
                if (result.ValueKind == JsonValueKind.Null) return null;
                return new UserProfile
                {
                    Username = result.TryGetProperty("username", out var u) ? u.GetString() : null,
                };
            }
            catch { return null; }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static FirebaseUser ParseUser(JsonElement el) => new()
        {
            Uid          = el.GetProperty("uid").GetString() ?? "",
            DisplayName  = el.TryGetProperty("displayName", out var dn)  ? dn.GetString() : null,
            Email        = el.TryGetProperty("email",       out var em)  ? em.GetString() : null,
            PhotoUrl     = el.TryGetProperty("photoURL",    out var ph)  ? ph.GetString() : null,
        };
    }

    public class FirebaseUser
    {
        public string  Uid         { get; set; } = "";
        public string? DisplayName { get; set; }
        public string? Email       { get; set; }
        public string? PhotoUrl    { get; set; }
    }

    public class UserProfile
    {
        public string? Username { get; set; }
        public bool IsComplete => !string.IsNullOrWhiteSpace(Username);
    }
}
