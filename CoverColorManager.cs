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
        //TODO: Implement automatic warmth/coldness detection
        /*
        private static float GetWarmDistance(UnityEngine.Color a, UnityEngine.Color b)
        {
            return (Mathf.Abs(a.r - b.r)+Mathf.Abs(a.g - b.g)*0.5f+Mathf.Abs(a.b - b.b)*0.5f)/3f;
        }

        private static float GetColdDistance(UnityEngine.Color a, UnityEngine.Color b)
        {
            return (Mathf.Abs(a.r - b.r)*0.5f+Mathf.Abs(a.g - b.g)*0.5f+Mathf.Abs(a.b - b.b))/3f;
        }

        private static UnityEngine.Color GetWarmestColor(List<QuantizedColor> colors)
        {
            var warmestColor = new UnityEngine.Color(0f, 128f, 255f);
            foreach (var t in colors)
            {
                var dist = GetWarmDistance(warmestColor, t.UnityColor);
                var dist2 = GetWarmDistance(UnityEngine.Color.red, warmestColor);
                if (dist < dist2)
                {
                    warmestColor = t.UnityColor;
                }
            }
            return warmestColor;
        }
        
        internal static UnityEngine.Color GetColdestColor(List<QuantizedColor> colors)
        {
            var warmestColor = new UnityEngine.Color(255f, 128f, 0f);
            foreach (var t in colors)
            {
                var dist = GetWarmDistance(warmestColor, t.UnityColor);
                var dist2 = GetWarmDistance(UnityEngine.Color.blue, warmestColor);
                if (dist < dist2)
                {
                    warmestColor = t.UnityColor;
                }
            }
            return warmestColor;
        }*/
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
            //var leftColor = GetWarmestColor(colors);
            //var rightColor = GetColdestColor(colors);
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
