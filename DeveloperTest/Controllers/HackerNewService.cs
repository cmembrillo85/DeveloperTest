using DeveloperTest.Service;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HackerNewsController : ControllerBase
    {
        private readonly HackerNewsService _hackerNewsService;

        public HackerNewsController(HackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        [HttpGet("top/{n}")]
        public async Task<IActionResult> GetTopStories(int n)
        {
            var storyIds = await _hackerNewsService.GetBestStoryIdsAsync();
            var tasks = storyIds.Take(n).Select(id => _hackerNewsService.GetStoryDetailsAsync(id));
            var stories = await Task.WhenAll(tasks);

            var sortedStories = stories.OrderByDescending(s => s.Score).ToList();

            return Ok(sortedStories);
        }
    }
}