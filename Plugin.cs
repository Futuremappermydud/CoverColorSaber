using CoverColorSaber.Configuration;
using CoverColorSaber.Installers;
using IPA;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

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
