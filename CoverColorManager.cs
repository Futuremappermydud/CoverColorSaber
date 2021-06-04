using System.Collections.Generic;
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

            var leftColor = colors[0].UnityColor;
            var rightColor = colors[1].UnityColor;
            var obsColor = colors[2].UnityColor;

            var scheme = new ColorScheme("CoverSaber", "Cover Saber", true, "Cover Saber", true, leftColor, rightColor, leftColor, rightColor, false, leftColor, rightColor, obsColor);
            
            Cache.TryAdd(levelID, scheme);
            PaletteCache.TryAdd(levelID, colors);
            result.Scheme = scheme;
            result.Colors = colors;
            return result;
        }
    }
}
