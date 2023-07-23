using CoverColorSaber.AffinityPatches;
using CoverColorSaber.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace CoverColorSaber.Installers
{
	internal class CoverColorSaberAppInstaller : Installer
	{
		public override void InstallBindings()
		{
			Container.Bind<CacheManager>().AsSingle().NonLazy();
			Container.BindInterfacesTo<SetColorSchemePatch>().AsSingle(); //Affinity Patch
		}
	}
}
