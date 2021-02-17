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
        static void Prefix(StandardLevelScenesTransitionSetupDataSO __instance, ref IDifficultyBeatmap difficultyBeatmap, ref ColorScheme overrideColorScheme)
        {
            if(Menu.instance.schemeEnabled)
            {
                CoverColorManager.Cache.TryGetValue(difficultyBeatmap.level.levelID, out overrideColorScheme);
            }
        }
    }
}