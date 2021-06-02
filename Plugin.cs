using IPA;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using BS_Utils.Utilities;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using static HMUI.ViewController;
using static PlayerSaveData;
using System.Threading.Tasks;
using HarmonyLib;
using System.Reflection;
using BeatSaberMarkupLanguage.GameplaySetup;
using System.Collections.Generic;

namespace CoverColorSaber
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; set; }

        [Init]
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Log.Info("CoverColorSaber initialized.");
            new Harmony("CoverSaber").PatchAll(Assembly.GetExecutingAssembly());
            GameplaySetup.instance.AddTab("Cover Color Saber", "CoverColorSaber.Settings.Panel.bsml", Menu.instance);
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
        static internal IPreviewBeatmapLevel last;
        public async void LevelSelected(LevelCollectionViewController lcvc, IPreviewBeatmapLevel level)
        {
            last = level;
            Texture2D tex = null;
            //Log.Info("Level Selected");
            tex = (await level.GetCoverImageAsync(System.Threading.CancellationToken.None)).texture;
            if (!(level is CustomPreviewBeatmapLevel) || tex == null)
            {
                RenderTexture tmp = RenderTexture.GetTemporary(
                                    tex.width,
                                    tex.height,
                                    0,
                                    RenderTextureFormat.Default,
                                    RenderTextureReadWrite.Linear);

                Graphics.Blit(tex, tmp);
                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = tmp;
                Texture2D myTexture2D = new Texture2D(tex.width, tex.height);
                myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                myTexture2D.Apply();
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(tmp);
                tex = myTexture2D;
            }

            ColorScheme scheme = new ColorScheme("CoverSaber", "Cover Saber", true, "Cover Saber", false, Color.white, Color.white, Color.white, Color.white, true, Color.white, Color.white, Color.white);
            List<ColorThief.QuantizedColor> colors = new List<ColorThief.QuantizedColor>();
            await Task.Run(() => { scheme = CoverColorManager.Cache.GetOrAdd(level.levelID, CoverColorManager.GetSchemeFromCoverImage(tex, level.levelID, out colors)); });
            Log.Info("Colors " + colors.Count.ToString());
            Menu.instance.songName = level.songName;
            Menu.instance.SetColors(colors, scheme, tex, level.levelID);
        }
    }
}
