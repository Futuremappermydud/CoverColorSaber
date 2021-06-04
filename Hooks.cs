using BS_Utils.Utilities;
using HarmonyLib;
using HMUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoverColorSaber
{
    [HarmonyPatch(typeof(StandardLevelScenesTransitionSetupDataSO), "Init")]
    public class SetColorSchemePatch
    {
        private static void Prefix(ref IDifficultyBeatmap difficultyBeatmap, ref ColorScheme overrideColorScheme)
        {
            if(Settings.Menu.instance.SchemeEnabled)
            {
                CoverColorManager.Cache.TryGetValue(difficultyBeatmap.level.levelID, out overrideColorScheme);
            }
        }
    }
}