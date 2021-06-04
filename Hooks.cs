using HarmonyLib;

namespace CoverColorSaber
{
    [HarmonyPatch(typeof(StandardLevelScenesTransitionSetupDataSO), "Init")]
    public class SetColorSchemePatch
    {
        // ReSharper disable once UnusedMember.Local
        private static void Prefix(ref IDifficultyBeatmap difficultyBeatmap, ref ColorScheme overrideColorScheme)
        {
            if(Settings.Menu.instance.SchemeEnabled)
            {
                CoverColorManager.Cache.TryGetValue(difficultyBeatmap.level.levelID, out overrideColorScheme);
            }
        }
    }
}