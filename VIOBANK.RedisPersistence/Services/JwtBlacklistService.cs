using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIOBANK.RedisPersistence.Services
{
    public class JwtBlacklistService
    {
        private readonly IDatabase _redisDb;
        private readonly TimeSpan _tokenTtl = TimeSpan.FromHours(12);

        public JwtBlacklistService(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task AddToBlacklistAsync(string token)
        {
            await _redisDb.StringSetAsync($"blacklist:{token}", "blacklisted", _tokenTtl);
        }

        public async Task<bool> IsBlacklistedAsync(string token)
        {
            return await _redisDb.KeyExistsAsync($"blacklist:{token}");
        }
    }
}
