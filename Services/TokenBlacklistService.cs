using System.Collections.Concurrent;

namespace WebApi.Services
{
    public class TokenBlacklistService
    {
        private readonly ConcurrentDictionary<string, DateTime> _revokedTokens = new();

        public void RevokeToken(string token, DateTime expiration)
        {
            _revokedTokens[token] = expiration;
        }

        public bool IsTokenRevoked(string token)
        {
            return _revokedTokens.TryGetValue(token, out var expiration) && expiration > DateTime.UtcNow;
        }
    }
}
