using CoverColorSaber.Managers;
using CoverColorSaber.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace CoverColorSaber.Installers
{
	internal class CoverColorSaberMenuInstaller : Installer
	{
		public override void InstallBindings()
		{
			Container.Bind<CoverColorManager>().AsSingle().NonLazy();
			Container.Bind<SettingsMenu>().AsSingle().NonLazy();
		}
	}
}
