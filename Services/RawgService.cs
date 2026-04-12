using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace PlayedGames.Services
{
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
            catch
            {
                return null;
            }
        }
    }

    file class RawgResponse
    {
        [JsonPropertyName("results")]
        public List<RawgGame>? Results { get; set; }
    }

    file class RawgGame
    {
        [JsonPropertyName("background_image")]
        public string? BackgroundImage { get; set; }
    }
}
