using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Concurrent;
using ColorThief;
using Color = UnityEngine.Color;

namespace CoverColorSaber
{
    public class ColorDataResult
    {
        public ColorScheme Scheme;
        public List<QuantizedColor> Colors;
    }
    //meh monkas class
    internal static class CoverColorManager
    {
        internal static readonly ConcurrentDictionary<string, ColorScheme> Cache = new ConcurrentDictionary<string, ColorScheme>();
        private static readonly ConcurrentDictionary<string, List<QuantizedColor>> PaletteCache = new ConcurrentDictionary<string, List<QuantizedColor>>();
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

            Color LeftColor;
            Color RightColor;
            Color ObsColor;

            var thief = new ColorThief.ColorThief();
            var colors = new List<QuantizedColor>();

            await Task.Run(() => { colors = thief.GetPalette(tex); });

            LeftColor = colors[0].UnityColor;
            RightColor = colors[1].UnityColor;
            ObsColor = colors[2].UnityColor;

            var scheme = new ColorScheme("CoverSaber", "Cover Saber", true, "Cover Saber", true, LeftColor, RightColor, LeftColor, RightColor, false, LeftColor, RightColor, ObsColor);
            
            Cache.TryAdd(levelID, scheme);
            PaletteCache.TryAdd(levelID, colors);
            result.Scheme = scheme;
            result.Colors = colors;
            return result;
        }
    }
}
