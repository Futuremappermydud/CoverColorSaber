using CoverColorSaber.Configuration;
using CoverColorSaber.Models;
using System.Collections.Concurrent;
using System.Linq;

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

		public void SetConfigCache(string hash, ColorDataResult result)
		{
			PluginConfig.Instance.PaletteCache[hash] = result.Colors.Select(x => new ColorSerialized(x.UnityColor)).ToList();
			PluginConfig.Instance.SchemeCache[hash] = new System.Collections.Generic.List<ColorCacheParent> {
				new ColorCacheParent("SaberA", new ColorSerialized(result.Scheme.saberAColor)),
				new ColorCacheParent("SaberB", new ColorSerialized(result.Scheme.saberBColor)),
				new ColorCacheParent("Obstacales", new ColorSerialized(result.Scheme.obstaclesColor)),
			};
		}

		public ColorDataResult GetOrAdd(string hash, ColorDataResult result)
		{
			SetCache(hash, result);
			return result;
		}
	}
}