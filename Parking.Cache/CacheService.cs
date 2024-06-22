using Microsoft.Extensions.Caching.Distributed;
using Parking.Core.Cache;
using Parking.Core.DataBase;

namespace Parking.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IDataBaseService _dataBaseService;
        public CacheService(IDataBaseService dataBaseService, IDistributedCache cache)
        {
            _cache = cache;
            _dataBaseService = dataBaseService;
        }
    }
}
