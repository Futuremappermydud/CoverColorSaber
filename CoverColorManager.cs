using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Concurrent;
using ColorThief;
using Color = UnityEngine.Color;

namespace CoverColorSaber
{
    public class ColorDataResult
    {
        public ColorScheme scheme;
        public List<QuantizedColor> colors;
    }
    //meh monkas class
    static class CoverColorManager
    {
        internal static ConcurrentDictionary<string, ColorScheme> Cache = new ConcurrentDictionary<string, ColorScheme>();
        internal static ConcurrentDictionary<string, List<QuantizedColor>> QuantCache = new ConcurrentDictionary<string, List<QuantizedColor>>();
        public async static Task<ColorDataResult> GetSchemeFromCoverImage(Texture2D tex, string levelID)
        {
            ColorDataResult result = new ColorDataResult();
            var quantizedColors = new List<QuantizedColor>();
            if (QuantCache.ContainsKey(levelID))
            {
                QuantCache.TryGetValue(levelID, out quantizedColors);
            }
            if (Cache.ContainsKey(levelID))
            {
                result.colors = quantizedColors;
                result.scheme = Cache[levelID];
                return result;
            }

            Color LeftColor;
            Color RightColor;
            Color ObsColor;

            ColorThief.ColorThief thief = new ColorThief.ColorThief();
            List<QuantizedColor> colors = new List<QuantizedColor>();

            await Task.Run(() => { colors = thief.GetPalette(tex); });

            LeftColor = colors[0].UnityColor;
            RightColor = colors[1].UnityColor;
            ObsColor = colors[2].UnityColor;

            ColorScheme scheme = new ColorScheme("CoverSaber", "Cover Saber", true, "Cover Saber", true, LeftColor, RightColor, LeftColor, RightColor, false, LeftColor, RightColor, ObsColor);
            
            Cache.TryAdd(levelID, scheme);
            QuantCache.TryAdd(levelID, colors);
            quantizedColors = colors;
            result.scheme = scheme;
            result.colors = colors;
            return result;
        }
    }
}
