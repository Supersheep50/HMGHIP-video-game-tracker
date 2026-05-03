using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace PlayedGames.Services
{
    public record RawgSearchResult(string Name, string? ArtUrl, string? Genre);

    public record RawgUpcomingGame(string Name, string? ArtUrl, string? Genre, string? Released);

    public record RawgGameDetail(
        string        Name,
        string?       ArtUrl,
        string?       Genre,
        string?       Description,
        string?       Released,
        string?       Developer,
        string?       Publisher,
        List<string>  Platforms,
        double?       Rating,
        int?          Metacritic);

    public class RawgService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        // Per-session caches — Blazor WASM has one user per process, so simple dicts suffice.
        // Key is lowercased trimmed name. Values may be null (negative cache).
        private readonly ConcurrentDictionary<string, Task<string?>>          _artCache     = new();
        private readonly ConcurrentDictionary<string, Task<RawgGameDetail?>>  _detailCache  = new();
        private readonly ConcurrentDictionary<string, Task<List<RawgSearchResult>>> _searchCache = new();
        private Task<List<RawgSearchResult>>?    _popularCache;
        private Task<List<RawgUpcomingGame>>?    _upcomingCache;

        public RawgService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["Rawg:ApiKey"] ?? "";
        }

        private static string CacheKey(string s) => s.Trim().ToLowerInvariant();

        public Task<string?> GetGameArtAsync(string? gameName)
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(gameName))
                return Task.FromResult<string?>(null);

            return _artCache.GetOrAdd(CacheKey(gameName), async _ =>
            {
                var url = $"https://api.rawg.io/api/games?key={_apiKey}&search={Uri.EscapeDataString(gameName)}&page_size=1&search_precise=true";
                try
                {
                    var response = await _http.GetFromJsonAsync<RawgResponse>(url);
                    return response?.Results?.FirstOrDefault()?.BackgroundImage;
                }
                catch { return null; }
            });
        }

        public Task<List<RawgSearchResult>> GetPopularGamesAsync(int pageSize = 20)
        {
            if (string.IsNullOrEmpty(_apiKey)) return Task.FromResult(new List<RawgSearchResult>());

            return _popularCache ??= LoadPopularAsync(pageSize);
        }

        private async Task<List<RawgSearchResult>> LoadPopularAsync(int pageSize)
        {
            var url = $"https://api.rawg.io/api/games?key={_apiKey}&ordering=-added&page_size={pageSize}";
            try
            {
                var response = await _http.GetFromJsonAsync<RawgResponse>(url);
                return response?.Results?
                    .Where(r => !string.IsNullOrEmpty(r.Name))
                    .Select(r => new RawgSearchResult(
                        r.Name!,
                        r.BackgroundImage,
                        r.Genres?.FirstOrDefault()?.Name))
                    .ToList() ?? new();
            }
            catch { return new(); }
        }

        public Task<List<RawgUpcomingGame>> GetUpcomingGamesAsync(int pageSize = 15)
        {
            if (string.IsNullOrEmpty(_apiKey)) return Task.FromResult(new List<RawgUpcomingGame>());

            return _upcomingCache ??= LoadUpcomingAsync(pageSize);
        }

        private async Task<List<RawgUpcomingGame>> LoadUpcomingAsync(int pageSize)
        {
            var from = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");
            var to   = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-dd");
            var url  = $"https://api.rawg.io/api/games?key={_apiKey}&dates={from},{to}&ordering=released&page_size={pageSize}";
            try
            {
                var response = await _http.GetFromJsonAsync<RawgResponse>(url);
                return response?.Results?
                    .Where(r => !string.IsNullOrEmpty(r.Name))
                    .Select(r => new RawgUpcomingGame(
                        r.Name!,
                        r.BackgroundImage,
                        r.Genres?.FirstOrDefault()?.Name,
                        r.Released))
                    .ToList() ?? new();
            }
            catch { return new(); }
        }

        public Task<RawgGameDetail?> GetGameDetailsAsync(string gameName)
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(gameName))
                return Task.FromResult<RawgGameDetail?>(null);

            return _detailCache.GetOrAdd(CacheKey(gameName), async _ =>
            {
                try
                {
                    var searchUrl = $"https://api.rawg.io/api/games?key={_apiKey}&search={Uri.EscapeDataString(gameName)}&page_size=1&search_precise=true";
                    var search = await _http.GetFromJsonAsync<RawgResponse>(searchUrl);
                    var slug = search?.Results?.FirstOrDefault()?.Slug;
                    if (string.IsNullOrEmpty(slug)) return null;

                    var detailUrl = $"https://api.rawg.io/api/games/{slug}?key={_apiKey}";
                    var d = await _http.GetFromJsonAsync<RawgFullGame>(detailUrl);
                    if (d == null) return null;

                    return new RawgGameDetail(
                        Name:        d.Name ?? gameName,
                        ArtUrl:      d.BackgroundImage,
                        Genre:       d.Genres?.FirstOrDefault()?.Name,
                        Description: StripHtml(d.DescriptionRaw ?? d.Description),
                        Released:    d.Released,
                        Developer:   d.Developers?.FirstOrDefault()?.Name,
                        Publisher:   d.Publishers?.FirstOrDefault()?.Name,
                        Platforms:   d.Platforms?
                                        .Select(p => p.Platform?.Name ?? "")
                                        .Where(s => !string.IsNullOrEmpty(s))
                                        .ToList() ?? new(),
                        Rating:      d.Rating,
                        Metacritic:  d.Metacritic);
                }
                catch { return null; }
            });
        }

        private static string? StripHtml(string? input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            // RAWG description_raw is plain; description is HTML. Quick tag strip just in case.
            return System.Text.RegularExpressions.Regex.Replace(input, "<[^>]+>", "").Trim();
        }

        public Task<List<RawgSearchResult>> SearchGamesAsync(string query)
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(query))
                return Task.FromResult(new List<RawgSearchResult>());

            return _searchCache.GetOrAdd(CacheKey(query), async _ =>
            {
                var url = $"https://api.rawg.io/api/games?key={_apiKey}&search={Uri.EscapeDataString(query)}&page_size=12";
                try
                {
                    var response = await _http.GetFromJsonAsync<RawgResponse>(url);
                    return response?.Results?
                        .Where(r => !string.IsNullOrEmpty(r.Name))
                        .Select(r => new RawgSearchResult(
                            r.Name!,
                            r.BackgroundImage,
                            r.Genres?.FirstOrDefault()?.Name))
                        .ToList() ?? new();
                }
                catch { return new(); }
            });
        }
    }

    file class RawgResponse
    {
        [JsonPropertyName("results")]
        public List<RawgGame>? Results { get; set; }
    }

    file class RawgGame
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("background_image")]
        public string? BackgroundImage { get; set; }

        [JsonPropertyName("genres")]
        public List<RawgGenre>? Genres { get; set; }

        [JsonPropertyName("released")]
        public string? Released { get; set; }
    }

    file class RawgGenre
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    file class RawgFullGame
    {
        [JsonPropertyName("name")]              public string? Name { get; set; }
        [JsonPropertyName("background_image")]  public string? BackgroundImage { get; set; }
        [JsonPropertyName("genres")]            public List<RawgGenre>? Genres { get; set; }
        [JsonPropertyName("description_raw")]   public string? DescriptionRaw { get; set; }
        [JsonPropertyName("description")]       public string? Description { get; set; }
        [JsonPropertyName("released")]          public string? Released { get; set; }
        [JsonPropertyName("rating")]            public double? Rating { get; set; }
        [JsonPropertyName("metacritic")]        public int? Metacritic { get; set; }
        [JsonPropertyName("developers")]        public List<RawgNamed>? Developers { get; set; }
        [JsonPropertyName("publishers")]        public List<RawgNamed>? Publishers { get; set; }
        [JsonPropertyName("platforms")]         public List<RawgPlatformWrap>? Platforms { get; set; }
    }

    file class RawgNamed
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
    }

    file class RawgPlatformWrap
    {
        [JsonPropertyName("platform")] public RawgNamed? Platform { get; set; }
    }
}
