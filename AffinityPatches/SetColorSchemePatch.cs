using CoverColorSaber.Configuration;
using CoverColorSaber.Managers;
using HarmonyLib;
using SiraUtil.Affinity;

namespace CoverColorSaber.AffinityPatches
{
    public class SetColorSchemePatch : IAffinity
    {
        private CacheManager _cacheManager;
		public SetColorSchemePatch(CacheManager cacheManager)
		{
			_cacheManager = cacheManager;
			Plugin.Log.Info("bakldjfhj");
		}

		[AffinityPrefix]
		[AffinityPatch(typeof(StandardLevelScenesTransitionSetupDataSO), nameof(StandardLevelScenesTransitionSetupDataSO.Init))]
		public void Prefix(ref IDifficultyBeatmap difficultyBeatmap, ref ColorScheme overrideColorScheme)
        {
			Plugin.Log.Info("bruh");
			if (PluginConfig.Instance.Enabled && _cacheManager.TryGetCache(difficultyBeatmap.level.levelID, out var result))
			{
				Plugin.Log.Info("bruh2");
				overrideColorScheme = result.Scheme;
			}
		}
    }
}