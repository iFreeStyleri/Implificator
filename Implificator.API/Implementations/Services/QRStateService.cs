using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Implificator.Abstractions.Services;
using Implificator.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Implificator.API.Implementations.Services
{
    public class QRStateService : IQRStateService
    {
        private readonly IMemoryCache _cache;

        public QRStateService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void SetUserData(long userId, UserData sharedUser)
        {
            if (_cache.TryGetValue(userId, out UserData oldSharedUser))
                _cache.Remove(userId);
            _cache.Set(userId, sharedUser, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(20)));
        }

        public UserData? GetUserData(long userId)
        {
            if (_cache.TryGetValue(userId, out UserData sharedUser))
            {
                _cache.Remove(userId);
                return sharedUser;
            }
            return null;
        }
    }
}
