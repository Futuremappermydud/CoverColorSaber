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
    //meh monkas class
    static class CoverColorManager
    {
        internal static ConcurrentDictionary<string, ColorScheme> Cache = new ConcurrentDictionary<string, ColorScheme>();
        internal static ConcurrentDictionary<string, List<QuantizedColor>> QuantCache = new ConcurrentDictionary<string, List<QuantizedColor>>();
        public static float GetAverage(this Color col)
        {
            float avg = (col.r + col.g + col.b) / 3;
            return avg;
        }

        public static bool GetNearX(this Color col, float maxDist, float X)
        {
            float distr = Mathf.Abs(X - col.r);
            float distg = Mathf.Abs(X - col.g);
            float distb = Mathf.Abs(X - col.b);
            //Plugin.Log.Info("Distr: " + distr.ToString());
            //Plugin.Log.Info("Distg: " + distg.ToString());
            //Plugin.Log.Info("Distb: " + distb.ToString());
            bool ShouldRetTrue = false;
            if(distr < maxDist || distg < maxDist || distb < maxDist)
            {
                ShouldRetTrue = true;
            }   
            if(col.r == X || col.g == X || col.b == X)
            {
                ShouldRetTrue = true;   
            }
            return ShouldRetTrue;
        }
        public static float distanceAverage(this Color col1, Color col2)
        {
            float avg1 = col1.GetAverage();
            float avg2 = col2.GetAverage();
            return Math.Abs(avg1 - avg2);
        }
        public static List<Color> GetCutColors(List<Color> orig)
        {
            List<Color> New = new List<Color>();
            New = orig.Where((Color col) => { return col.distanceAverage(Color.white) > 25f || col.distanceAverage(Color.black) > 25f;}).ToList();
            return New;
        }

        public static List<Color> GetColors(Texture2D tex)
        {
            return tex.GetPixels().ToList();
        }
        public static ColorScheme GetSchemeFromCoverImage(Texture2D tex, string levelID, out List<QuantizedColor> quantizedColors)
        {
            quantizedColors = new List<QuantizedColor>();
            if (QuantCache.ContainsKey(levelID))
            {
                Plugin.Log.Info("Has Quant Cached");
                QuantCache.TryGetValue(levelID, out quantizedColors);
            }
            if (Cache.ContainsKey(levelID)) return Cache[levelID];
            Color LeftColor;
            Color RightColor;
            Color ObsColor;

            ColorThief.ColorThief thief = new ColorThief.ColorThief();
            List<QuantizedColor> colors;

            colors = thief.GetPalette(tex);

            LeftColor = colors[0].UnityColor;
            RightColor = colors[1].UnityColor;
            ObsColor = colors[2].UnityColor;

            ColorScheme scheme = new ColorScheme("CoverSaber", "Cover Saber", true, "Cover Saber", true, LeftColor, RightColor, LeftColor, RightColor, false, LeftColor, RightColor, ObsColor);
            
            Cache.TryAdd(levelID, scheme);
            QuantCache.TryAdd(levelID, colors);
            quantizedColors = colors;
            return scheme;
        }
    }
}
