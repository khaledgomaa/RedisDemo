using Microsoft.AspNetCore.Mvc;
using Redis.Demo.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Redis.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisConController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddToRedis(User user)
        {
            var redis = await ConnectionMultiplexer.ConnectAsync("localhost");

            var db = redis.GetDatabase();

            string userJson = JsonSerializer.Serialize(user);

            await db.StringSetAsync($"user:1", userJson, when: When.NotExists);

            return Ok(new { Message = "Successfully added new user to redis" });
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var redis = await ConnectionMultiplexer.ConnectAsync("localhost");

            var db = redis.GetDatabase();

            var user = await db.StringGetAsync("user:1");

            var fetchedUser = JsonSerializer.Deserialize<User>(user);

            return Ok(fetchedUser);
        }

        [HttpGet("ping")]
        public async Task<IActionResult> PingRedis()
        {
            var redis = await ConnectionMultiplexer.ConnectAsync("localhost");

            var db = redis.GetDatabase();

            var result = await db.PingAsync();

            return Ok(result);
        }

        [HttpGet("pushToList")]
        public async Task<IActionResult> PushToList(string key, string[] data)
        {
            var redis = await ConnectionMultiplexer.ConnectAsync("localhost");

            var db = redis.GetDatabase();

            await db.ListLeftPushAsync(key, data.Select(x => (RedisValue)x).ToArray());

            return Ok(new { Message = "Successfully pushed the list" });
        }
    }
}
