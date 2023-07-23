using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using ColorThief;
using CoverColorSaber.Configuration;
using CoverColorSaber.Managers;
using HMUI;
using IPA.Utilities;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CoverColorSaber.Settings
{
	public class SettingsMenu
	{
		[UIComponent("song-name-text")]
		private TextMeshProUGUI songNameText = null;

		[UIObject("all-colors")]
		private GameObject AllColors = null;

		[UIComponent("set-button")]
		private Button SetButton = null;

		[UIValue("enabled")]
		public bool SchemeEnabled
		{
			get => PluginConfig.Instance.Enabled;
			set => PluginConfig.Instance.Enabled = value;
		}

		[UIObject("colors")]
		private readonly GameObject colors = null;

		private readonly Dictionary<string, ImageView> colorImages = new Dictionary<string, ImageView>();
		private readonly Dictionary<string, ImageView> colorIcons = new Dictionary<string, ImageView>();
		private ColorScheme scheme;
		private string currentlevelID;
		private int currentPaletteIndex;
		private RectTransform paletteRect;
		private GameObject templateImg;
		private GameObject obsTemplateImg;
		private List<QuantizedColor> currentPaletteColors = new List<QuantizedColor>();
		private string selected = "saberA";

		private Dictionary<string, ImageView> colImages = new Dictionary<string, ImageView>();
		private ToggleGroup paletteToggleGroup;
		private ToggleGroup colorsToggleGroup;

		private HoverHintController _hoverHintController;
		private CacheManager _cacheManager;

		[Inject]
		private void Construct(HoverHintController hoverHintController, CacheManager cacheManager)
		{
			_hoverHintController = hoverHintController;
			_cacheManager = cacheManager;
			GameplaySetup.instance.AddTab("Cover Color Saber", "CoverColorSaber.UI.SettingsMenu.bsml", this);
		}

		[UIAction("set-val")]
		public void SetVal()
		{
			if (currentPaletteColors.Count < 1) return;
			scheme.SetField("_" + selected + "Color", currentPaletteColors[currentPaletteIndex].UnityColor);
			SetColors(currentPaletteColors ?? new List<QuantizedColor>(), scheme, currentlevelID);
			_cacheManager.SetCache(currentlevelID, new Models.ColorDataResult(scheme, currentPaletteColors));

		}
		public void ToggleSelected(bool value, Toggle tgle)
		{
			if (!value) return;
			selected = tgle.gameObject.name;
		}
		public void SetSongName(string value)
		{
			songNameText.text = value;
			SetButton.enabled = !string.IsNullOrEmpty(value);
		}
		public void SetColors(List<QuantizedColor> paletteColors, ColorScheme setScheme, string levelID)
		{
			currentlevelID = levelID;
			currentPaletteColors = paletteColors;
			scheme = setScheme;

			if (colorImages.Count == 0)
			{
				if (templateImg == null)
				{
					var orig = Resources.FindObjectsOfTypeAll<EditColorSchemeController>().First(x => x.name == "EditColorSchemeController").transform.Find("Content").Find("ColorSchemeColorsToggleGroup");
					templateImg = GameObject.Instantiate(orig.Find("SaberA").gameObject);
					obsTemplateImg = GameObject.Instantiate(orig.Find("Obstacles").gameObject);
					GameObject.DontDestroyOnLoad(templateImg);
					GameObject.DontDestroyOnLoad(obsTemplateImg);
				}

				var layout = colors.GetComponent<HorizontalLayoutGroup>();
				layout.childForceExpandWidth = false;
				layout.childForceExpandHeight = false;
				layout.childControlHeight = false;
				layout.childControlWidth = false;
				layout.childAlignment = TextAnchor.MiddleCenter;
				layout.spacing = 2.5f;
				colors.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

				paletteRect = AllColors.GetComponent<RectTransform>();
				var vert = AllColors.GetComponent<VerticalLayoutGroup>();
				vert.childAlignment = TextAnchor.UpperCenter;
				vert.spacing = 0;
				vert.childForceExpandWidth = false;
				vert.childForceExpandHeight = false;
				vert.childControlHeight = false;
				vert.childControlWidth = false;

				paletteToggleGroup = paletteRect.gameObject.AddComponent<ToggleGroup>();
				colorsToggleGroup = colors.AddComponent<ToggleGroup>();

				colImages = new Dictionary<string, ImageView>();

				CreateColorImage("saberA", "Left Saber/Left Lights Color", templateImg);
				CreateColorImage("saberB", "Right Saber/Right Lights Color", templateImg);
				CreateColorImage("obstacles", "Obstacle Color", obsTemplateImg);

				CreatePaletteToggle("col1", 0);
				CreatePaletteToggle("col2", 1);
				CreatePaletteToggle("col3", 2);
				CreatePaletteToggle("col4", 3);
				CreatePaletteToggle("col5", 4);

				paletteToggleGroup.SetAllTogglesOff();
			}

			colImages["col1"].color = currentPaletteColors[0].UnityColor;
			colImages["col2"].color = currentPaletteColors[1].UnityColor;
			colImages["col3"].color = currentPaletteColors[2].UnityColor;
			colImages["col4"].color = currentPaletteColors[3].UnityColor;
			colImages["col5"].color = currentPaletteColors[4].UnityColor;

			colorImages["saberA"].color = scheme.saberAColor;
			colorImages["saberB"].color = scheme.saberBColor;
			colorImages["obstacles"].color = scheme.obstaclesColor;

			colorIcons["saberA"].color = UnityEngine.Color.white;
			colorIcons["saberB"].color = UnityEngine.Color.white;
			colorIcons["obstacles"].color = UnityEngine.Color.white;
		}
		private void CreateColorImage(string name, string hoverText, GameObject template)
		{
			var templateParent = template.transform;
			var templateIcon = template.transform.Find("Icon").GetComponent<ImageView>();

			var parent = GameObject.Instantiate(templateParent, colors.transform, false);
			var image = parent.transform.Find("ColorImage").GetComponent<ImageView>();
			var hover = parent.gameObject.AddComponent<HoverHint>();
			hover.text = hoverText;
			hover.SetField("_hoverHintController", _hoverHintController);
			var toggle = parent.GetComponent<Toggle>();
			toggle.isOn = false;
			toggle.group = colorsToggleGroup;
			toggle.onValueChanged.AddListener(value => ToggleSelected(value, toggle));
			parent.gameObject.name = name;

			var icon = parent.Find("Icon").GetComponent<ImageView>();
			icon.sprite = templateIcon.sprite;

			colorImages[name] = image;
			colorIcons[name] = icon;
		}
		private void CreatePaletteToggle(string name, int index)
		{
			var parent = GameObject.Instantiate(templateImg, paletteRect.transform, false);
			GameObject.Destroy(parent.transform.Find("Icon").gameObject);
			var image = parent.transform.Find("ColorImage").GetComponent<ImageView>();
			image.sprite = colorImages["saberA"].sprite;
			image.material = colorImages["saberA"].material;
			var toggle = parent.GetComponent<Toggle>();
			toggle.group = paletteToggleGroup;
			toggle.onValueChanged.AddListener(value => { if (value) currentPaletteIndex = index; });
			parent.gameObject.name = name;

			colImages[name] = image;
		}
	}
}