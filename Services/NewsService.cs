using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace PlayedGames.Services
{
    public record NewsItem(string Title, string Link, string Source, DateTime? Published, string? ImageUrl);

    // Gaming headlines from major outlets' RSS feeds, fetched through rss2json
    // (browser-side RSS is CORS-blocked; rss2json proxies with Access-Control-Allow-Origin: *).
    public class NewsService
    {
        private readonly HttpClient _http;
        private Task<List<NewsItem>>? _cache;

        private static readonly (string Source, string FeedUrl)[] Feeds =
        {
            ("IGN",       "https://feeds.feedburner.com/ign/games-all"),
            ("GameSpot",  "https://www.gamespot.com/feeds/game-news"),
            ("Eurogamer", "https://www.eurogamer.net/feed/news"),
        };

        public NewsService(HttpClient http) => _http = http;

        public Task<List<NewsItem>> GetHeadlinesAsync() => _cache ??= LoadAsync();

        private async Task<List<NewsItem>> LoadAsync()
        {
            var tasks = Feeds.Select(f => LoadFeedAsync(f.Source, f.FeedUrl));
            var results = await Task.WhenAll(tasks);

            return results
                .SelectMany(r => r)
                .OrderByDescending(n => n.Published ?? DateTime.MinValue)
                .ToList();
        }

        private async Task<List<NewsItem>> LoadFeedAsync(string source, string feedUrl)
        {
            try
            {
                var url = $"https://api.rss2json.com/v1/api.json?rss_url={Uri.EscapeDataString(feedUrl)}";
                var resp = await _http.GetFromJsonAsync<Rss2JsonResponse>(url);
                if (resp?.Status != "ok" || resp.Items == null) return new();

                return resp.Items
                    .Where(i => !string.IsNullOrWhiteSpace(i.Title) && !string.IsNullOrWhiteSpace(i.Link))
                    .Select(i => new NewsItem(
                        i.Title!.Trim(),
                        i.Link!,
                        source,
                        DateTime.TryParse(i.PubDate, out var dt) ? dt : null,
                        FirstNonEmpty(i.Thumbnail, i.Enclosure?.Link)))
                    .ToList();
            }
            catch { return new(); }
        }

        private static string? FirstNonEmpty(params string?[] values) =>
            values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        private class Rss2JsonResponse
        {
            [JsonPropertyName("status")] public string? Status { get; set; }
            [JsonPropertyName("items")]  public List<Rss2JsonItem>? Items { get; set; }
        }

        private class Rss2JsonItem
        {
            [JsonPropertyName("title")]     public string? Title { get; set; }
            [JsonPropertyName("link")]      public string? Link { get; set; }
            [JsonPropertyName("pubDate")]   public string? PubDate { get; set; }
            [JsonPropertyName("thumbnail")] public string? Thumbnail { get; set; }
            [JsonPropertyName("enclosure")] public Rss2JsonEnclosure? Enclosure { get; set; }
        }

        private class Rss2JsonEnclosure
        {
            [JsonPropertyName("link")] public string? Link { get; set; }
        }
    }
}
