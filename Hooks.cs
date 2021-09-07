using HarmonyLib;

namespace CoverColorSaber
{
    [HarmonyPatch(typeof(StandardLevelScenesTransitionSetupDataSO), "Init")]
    public class SetColorSchemePatch
    {
        // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
        private static void Prefix(ref IDifficultyBeatmap difficultyBeatmap, ref ColorScheme overrideColorScheme)
#pragma warning restore IDE0051 // Remove unused private members
        {
            if(Settings.Menu.instance.SchemeEnabled)
            {
                CoverColorManager.Cache.TryGetValue(difficultyBeatmap.level.levelID, out overrideColorScheme);
            }
        }
    }
}