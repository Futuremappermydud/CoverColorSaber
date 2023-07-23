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
using SiraUtil.Zenject;
using CoverColorSaber.Installers;

namespace CoverColorSaber
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static IPALogger Log { get; set; }

        [Init]
        public void Init(IPALogger logger, IPA.Config.Config config, Zenjector zenjector)
        {
            PluginConfig.Instance = config.Generated<PluginConfig>();
            Log = logger;
            zenjector.Install<CoverColorSaberAppInstaller>(Location.App);
            zenjector.Install<CoverColorSaberMenuInstaller>(Location.Menu);
            zenjector.Install<CoverColorSaberGameInstaller>(Location.StandardPlayer);
        }
    }
}
