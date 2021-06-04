using IPA;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using BS_Utils.Utilities;
using System.Threading.Tasks;
using HarmonyLib;
using System.Reflection;
using BeatSaberMarkupLanguage.GameplaySetup;
using System.Collections.Generic;
using IPA.Config.Stores;

namespace CoverColorSaber
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        private static IPALogger Log { get; set; }

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

        private static Texture2D GetFromUnreadable(Texture2D tex)
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
            var myTexture2D = new Texture2D(tex.width, tex.height);
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);
            return myTexture2D;
        }

        private static async void LevelSelected(LevelCollectionViewController lcvc, IPreviewBeatmapLevel level)
        {
            Texture2D tex;
            try
            {
                tex = (await level.GetCoverImageAsync(System.Threading.CancellationToken.None)).texture;
            }
            catch
            {
                tex = GetFromUnreadable((level as CustomPreviewBeatmapLevel)?.defaultCoverImage.texture);
            }
            if (!(level is CustomPreviewBeatmapLevel) || tex == null || !tex.isReadable)
            {
                tex = GetFromUnreadable(tex);
            }

            var scheme = new ColorScheme("CoverSaber", "Cover Saber", true, "Cover Saber", false, Color.white, Color.white, Color.white, Color.white, true, Color.white, Color.white, Color.white);
            var colors = new List<ColorThief.QuantizedColor>();
            await Task.Run(async () => { var data = await CoverColorManager.GetSchemeFromCoverImage(tex, level.levelID); scheme = CoverColorManager.Cache.GetOrAdd(level.levelID, data.Scheme); colors = data.Colors; });

            Settings.Menu.instance.SongName = level.songName;
            Settings.Menu.instance.SetColors(colors, scheme, tex, level.levelID);
        }
    }
}
