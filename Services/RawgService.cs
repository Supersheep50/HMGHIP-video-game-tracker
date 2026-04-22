using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace PlayedGames.Services
{
    public record RawgSearchResult(string Name, string? ArtUrl, string? Genre);

    public class RawgService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public RawgService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["Rawg:ApiKey"] ?? "";
        }

        public async Task<string?> GetGameArtAsync(string? gameName)
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(gameName))
                return null;

            var url = $"https://api.rawg.io/api/games?key={_apiKey}&search={Uri.EscapeDataString(gameName)}&page_size=1&search_precise=true";

            try
            {
                var response = await _http.GetFromJsonAsync<RawgResponse>(url);
                return response?.Results?.FirstOrDefault()?.BackgroundImage;
            }
            catch { return null; }
        }

        public async Task<List<RawgSearchResult>> SearchGamesAsync(string query)
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(query))
                return new();

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

        [JsonPropertyName("background_image")]
        public string? BackgroundImage { get; set; }

        [JsonPropertyName("genres")]
        public List<RawgGenre>? Genres { get; set; }
    }

    file class RawgGenre
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
