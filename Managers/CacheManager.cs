using CoverColorSaber.Models;
using System.Collections.Concurrent;

namespace CoverColorSaber.Managers
{
	public class CacheManager
	{
		private static readonly ConcurrentDictionary<string, ColorDataResult> Cache = new();


		public bool TryGetCache(string hash, out ColorDataResult result)
		{
			return Cache.TryGetValue(hash, out result);
		}

		public void SetCache(string hash, ColorDataResult result)
		{
			Cache[hash] = result;
		}

		public ColorDataResult GetOrAdd(string hash, ColorDataResult result)
		{
			SetCache(hash, result);
			return result;
		}
	}
}