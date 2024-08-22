using DeveloperTest.Model;
using Microsoft.Extensions.Caching.Memory;

//using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace DeveloperTest.Service
{
    public class HackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public HackerNewsService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<List<int>> GetBestStoryIdsAsync()
        {
            var response = await _httpClient.GetStringAsync("https://hacker-news.firebaseio.com/v0/beststories.json");
            return JsonConvert.DeserializeObject<List<int>>(response);
        }

        public async Task<StoryResponse> GetStoryDetailsAsync(int storyId)
        {
            /*
            var response = await _httpClient.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json");
            var story = JsonConvert.DeserializeObject<HackerNewsStory>(response);

            return new StoryResponse
            {
                Title = story.Title,
                Uri = $"https://news.ycombinator.com/item?id={story.Id}",  // Construir la URL
                PostedBy = story.By,
                Time = DateTimeOffset.FromUnixTimeSeconds(story.Time).UtcDateTime,
                Score = story.Score,
                CommentCount = story.Descendants
            };*/

            if (!_cache.TryGetValue(storyId, out StoryResponse returnStory))
            {
                var response = await _httpClient.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json");
                var story = JsonConvert.DeserializeObject<HackerNewsStory>(response);

                returnStory = new StoryResponse
                {
                    Title = story.Title,
                    Uri = $"https://news.ycombinator.com/item?id={story.Id}",  // Construir la URL
                    PostedBy = story.By,
                    Time = DateTimeOffset.FromUnixTimeSeconds(story.Time).UtcDateTime,
                    Score = story.Score,
                    CommentCount = story.Descendants
                };

                // Configura el cache con un tiempo de vida (TTL) de 5 minutos
                _cache.Set(storyId, returnStory, TimeSpan.FromMinutes(5));
            }

            return returnStory;
        }
    }
}
