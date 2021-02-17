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
            BSEvents.earlyMenuSceneLoadedFresh += MenuLoadFresh;
            BSEvents.levelSelected += LevelSelected;
            new Harmony("CoverSaber").PatchAll(Assembly.GetExecutingAssembly());
        }
        [OnExit]
        public void OnApplicationStart()
        {
            BSEvents.lateMenuSceneLoadedFresh -= MenuLoadFresh;
            BSEvents.levelSelected -= LevelSelected;
        }

        private void MenuLoadFresh(ScenesTransitionSetupDataSO data)
        {
            GameplaySetup.instance.AddTab("Cover Color Saber", "CoverColorSaber.Settings.Panel.bsml", Menu.instance);
            /*var floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(120, 52f), false,
                                                                     new Vector3(0f, 0.05f, 1.4f),
                                                                     new Quaternion(90f, 0f, 0f, 0f));
            floatingScreen.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            panel = BeatSaberUI.CreateViewController<Panel>();
            floatingScreen.SetRootViewController(panel, AnimationType.In);*/
        }
        static internal IPreviewBeatmapLevel last;
        public async void LevelSelected(LevelCollectionViewController lcvc, IPreviewBeatmapLevel level)
        {
            last = level;
            Texture2D tex = null;
            //Log.Info("Level Selected");
            tex = (await level.GetCoverImageAsync(System.Threading.CancellationToken.None)).texture;
            if (!(level is CustomPreviewBeatmapLevel))
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

            ColorScheme scheme = new ColorScheme("CoverSaber", "Cover Saber", true, Color.white, Color.white, Color.white, Color.white, true, Color.white, Color.white, Color.white);
            await Task.Run(async ()=> { scheme = CoverColorManager.Cache.GetOrAdd(level.levelID, await CoverColorManager.GetSchemeFromCoverImage(tex, level.levelID)); });
            Menu.instance.songName = level.songName;
            Menu.instance.SetColors(scheme);
        }
    }
}
