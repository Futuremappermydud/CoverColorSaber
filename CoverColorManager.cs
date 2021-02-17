using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Concurrent;
using static PlayerSaveData;
using Random = UnityEngine.Random;

namespace CoverColorSaber
{
    static class CoverColorManager
    {
        internal static ConcurrentDictionary<string, ColorScheme> Cache = new ConcurrentDictionary<string, ColorScheme>();
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
        public static async Task<ColorScheme> GetSchemeFromCoverImage(Texture2D tex, string levelID)
        {
            
            List <Color> Colors = GetColors(tex);
            //Colors = GetCutColors(Colors);
            Plugin.Log.Info(Colors.Count.ToString());

            Color LastColor1 = Color.white;
            float LastAverage1 = 255;
            await Task.Run(() =>
             {
                for (int i = 0; i < Colors.Count; i++)
                {
                    Color CurrentColor = Colors[i];
                    float CurrentAverage = CurrentColor.GetAverage();
                    //Console.WriteLine(CurrentAverage.ToString() + " " + CurrentColor.name);
                    if (LastAverage1 > CurrentAverage && !LastColor1.Equals(CurrentColor) || GetNearX(CurrentColor * 255, 25, 0))
                    {
                        LastColor1 = CurrentColor;
                        LastAverage1 = CurrentAverage;
                    }
                }
             });

            Color LastColor2 = Color.black;
            float LastAverage2 = 0;
            await Task.Run(() =>
            {
                for (int i = 0; i < Colors.Count; i++)
                {
                    Color CurrentColor = Colors[i];
                    float CurrentAverage = CurrentColor.GetAverage();
                    //Console.WriteLine(CurrentAverage.ToString() + " " + CurrentColor.name);
                    if (LastAverage2 < CurrentAverage && !LastColor2.Equals(CurrentColor) || GetNearX(CurrentColor * 255, 25, 255))
                    {
                        LastColor2 = CurrentColor;
                        LastAverage2 = CurrentAverage;
                    }
                }
            });
            Plugin.Log.Info((LastColor1 * 255).ToString());
            Plugin.Log.Info((LastColor2 * 255).ToString());   
            Plugin.Log.Info(GetNearX(LastColor1 * 255, 25, 0).ToString());
            Plugin.Log.Info(GetNearX(LastColor2 * 255, 25, 255).ToString());
            ColorScheme scheme = new ColorScheme("CoverSaber", "Cover Saber", true, LastColor1, LastColor2, LastColor1, LastColor2, true, LastColor1, LastColor2, Colors[Colors.Count/2]);
            
            
            Cache.TryRemove(levelID, out _);
            Cache.TryAdd(levelID, scheme);
            return scheme;
        }
    }
}
