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
using HMUI;

namespace CoverColorSaber.UI.CustomTags
{
	internal class ColorToggleTag : BSMLTag
	{
		private GameObject prefab;
		public override string[] Aliases => new[] { "color-toggle" };
		public override GameObject CreateObject(Transform parent)
		{

			if (prefab == null)
			{
				var orig = Resources.FindObjectsOfTypeAll<EditColorSchemeController>().First(x => x.name == "EditColorSchemeController").transform.Find("Content").Find("ColorSchemeColorsToggleGroup");
				prefab = GameObject.Instantiate(orig.Find("SaberA").gameObject);
			}

			var img = GameObject.Instantiate(prefab, parent, false).transform.Find("ColorImage").GetComponent<ImageView>();
			var parent2 = img.transform.parent;
			parent2.gameObject.name = "Image";

			GameObject gameObject = new()
			{
				name = "CoverColorSaberToggleGroup"
			};
			gameObject.transform.parent = parent;

			var grp = gameObject.AddComponent<ToggleGroup>();
			gameObject.AddComponent<ExternalComponents>().components.Add(grp);

			return gameObject;
		}
	}
}
