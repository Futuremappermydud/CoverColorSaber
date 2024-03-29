﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Concurrent;
using ColorThief;

namespace CoverColorSaber
{
    public class ColorDataResult
    {
        public ColorScheme Scheme;
        public List<QuantizedColor> Colors;
    }
    
    internal static class CoverColorManager
    {
        internal static readonly ConcurrentDictionary<string, ColorScheme> Cache = new ConcurrentDictionary<string, ColorScheme>();
        private static readonly ConcurrentDictionary<string, List<QuantizedColor>> PaletteCache = new ConcurrentDictionary<string, List<QuantizedColor>>();

        private static float GetWarmth(UnityEngine.Color col)
        {
            float n = ((0.23881f) * col.r + (0.25499f) * col.g + (-0.58291f) * col.b) / ((0.11109f) * col.r + (-0.85406f) * col.g + (0.52289f) * col.b);
            float cct = (449f * Mathf.Pow(n, 3) + 3525f * Mathf.Pow(n, 2) + 6823.3f * n + 5520.33f);
            return cct;
        }
        public static async Task<ColorDataResult> GetSchemeFromCoverImage(Texture2D tex, string levelID)
        {
            var result = new ColorDataResult();
            var paletteColors = new List<QuantizedColor>();
            if (PaletteCache.ContainsKey(levelID))
            {
                PaletteCache.TryGetValue(levelID, out paletteColors);
            }
            if (Cache.ContainsKey(levelID))
            {
                result.Colors = paletteColors;
                result.Scheme = Cache[levelID];
                return result;
            }

            var thief = new ColorThief.ColorThief();
            var colors = new List<QuantizedColor>();

            await Task.Run(() => { colors = thief.GetPalette(tex); });
            colors.Sort((x, y) => y.Population.CompareTo(x.Population));
            var leftColor = colors[0].UnityColor;
            var rightColor = colors[1].UnityColor;
            var obsColor = colors[2].UnityColor;

            var scheme = new ColorScheme("CoverSaber", "Cover Saber", true, "Cover Saber", true, leftColor, rightColor, leftColor*1.3f, rightColor*1.3f, false, leftColor*1.5f, rightColor*1.5f, obsColor);
            
            Cache.TryAdd(levelID, scheme);
            PaletteCache.TryAdd(levelID, colors);
            result.Scheme = scheme;
            result.Colors = colors;
            return result;
        }
    }
}
