using IPA;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using BS_Utils.Utilities;
using System.Threading.Tasks;
using HarmonyLib;
using System.Reflection;
using BeatSaberMarkupLanguage.GameplaySetup;
using System.Collections.Generic;
using CoverColorSaber.Configuration;
using IPA.Config.Stores;

namespace CoverColorSaber
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static IPALogger Log { get; set; }

        [Init]
        public void Init(IPALogger logger, IPA.Config.Config config)
        {
            PluginConfig.Instance = config.Generated<PluginConfig>();
            Log = logger;
            Log.Info("CoverColorSaber initialized.");
            new Harmony("CoverSaber").PatchAll(Assembly.GetExecutingAssembly());
            GameplaySetup.instance.AddTab("Cover Color Saber", "CoverColorSaber.Settings.Panel.bsml", Settings.Menu.instance);
        }
        [OnExit]
        public void OnApplicationExit()
        {
            BSEvents.levelSelected -= LevelSelected;
        }

        [OnStart]
        public void OnApplicationStart()
        {
            BSEvents.levelSelected += LevelSelected;
        }

        private static Rect InvertAtlas(Rect i)
        {
            Rect o = new Rect(i.x, i.y, i.width, i.height);
            o.y = 2048 - o.y - 160;
            return o;
        }

        private static Texture2D GetFromUnreadable(Texture2D tex, Rect rect)
        {
            var tmp = RenderTexture.GetTemporary(
                                    tex.width,
                                    tex.height,
                                    0,
                                    RenderTextureFormat.Default,
                                    RenderTextureReadWrite.Linear);

            Graphics.Blit(tex, tmp);
            var previous = RenderTexture.active;
            RenderTexture.active = tmp;
            var myTexture2D = new Texture2D((int)rect.width, (int)rect.height);
            myTexture2D.ReadPixels(rect, 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);
            return myTexture2D;
        }

        private static async void LevelSelected(LevelCollectionViewController lcvc, IPreviewBeatmapLevel level)
        {
            Log.Info(level.songName);
            Texture2D tex;
            Sprite sprite = (await level.GetCoverImageAsync(System.Threading.CancellationToken.None));
            try
            {
                tex = sprite.texture;
            }
            catch
            {
                tex = GetFromUnreadable((level as CustomPreviewBeatmapLevel)?.defaultCoverImage.texture, sprite.textureRect);
            }
            if (!(level is CustomPreviewBeatmapLevel) || tex == null || !tex.isReadable)
            {
                tex = GetFromUnreadable(tex, InvertAtlas(sprite.textureRect));
            }

            Log.Info(sprite.textureRect.ToString());

            var scheme = new ColorScheme("CoverSaber", "Cover Saber", true, "Cover Saber", false, Color.white, Color.white, Color.white, Color.white, true, Color.white, Color.white, Color.white);
            var colors = new List<ColorThief.QuantizedColor>();
            await Task.Run(async () => { var data = await CoverColorManager.GetSchemeFromCoverImage(tex, level.levelID); scheme = CoverColorManager.Cache.GetOrAdd(level.levelID, data.Scheme); colors = data.Colors; });
            Log.Info(colors[0].UnityColor.ToString());
            System.IO.File.WriteAllBytes(System.IO.Path.Combine(IPA.Utilities.UnityGame.InstallPath, "bruh.png"), ImageConversion.EncodeToPNG(tex));
            Settings.Menu.instance.SongName = level.songName;
            Settings.Menu.instance.SetColors(colors, scheme, sprite, level.levelID);
        }
    }
}
