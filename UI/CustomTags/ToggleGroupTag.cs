using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using Polyglot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace CoverColorSaber.UI.CustomTags
{
	internal class ToggleGroupTag : BSMLTag
	{
		public override string[] Aliases => new[] { "toggle-group" };
		public override GameObject CreateObject(Transform parent)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "CoverColorSaberToggleGroup";
			gameObject.transform.parent = parent;

			var grp = gameObject.AddComponent<ToggleGroup>();
			gameObject.AddComponent<ExternalComponents>().components.Add(grp);

			return gameObject;
		}
	}
}
