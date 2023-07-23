using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using ColorThief;
using CoverColorSaber.Configuration;
using HMUI;
using IPA.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace CoverColorSaber.Settings
{
	public class SettingsMenu
    {
        [UIComponent("SongNameText")]
        private TextMeshProUGUI songNameText = null;

        [UIObject("toggle")]
        private GameObject toggle = null;

        [UIObject("AllColors")]
        private GameObject AllColors = null;

        [UIValue("songSelected")]
        public bool songSelected
        {
            get => songName != "";
        }
        private string songName;
        public string SongName
        {
            set
            {
                songName = value;
                songNameText.text = value;
            }
        }

        [UIValue("enabled")]
        public bool SchemeEnabled
        {
            get => PluginConfig.Instance.Enabled;
            set => PluginConfig.Instance.Enabled = value;
        }

        [UIObject("Colors")] private readonly GameObject colors = null;

        private ImageView leftImg;
        private ImageView rightImg;
        private ImageView obsImg;
        private ImageView leftImgIcon;
        private ImageView rightImgIcon;
        private ImageView obsImgIcon;
        private ColorScheme scheme;
        private string currentlevelID;
        private int currentPaletteIndex;
        private RectTransform paletteRect;
        private GameObject templateImg;
        private GameObject obsTemplateImg;
        private List<QuantizedColor> currentPaletteColors = new List<QuantizedColor>();
        private string selected = "saberA";

        private GameObject col1;
        private ImageView col1Img;
        private GameObject col2;
        private ImageView col2Img;
        private GameObject col3;
        private ImageView col3Img;
        private GameObject col4;
        private ImageView col4Img;
        private GameObject col5;
        private ImageView col5Img;

        [Inject]
        private void Construct()
        {
			GameplaySetup.instance.AddTab("Cover Color Saber", "CoverColorSaber.UI.SettingsMenu.bsml", this);
		}

        [UIAction("SetVal")]
        public void SetVal()
        {
            //bad
            if (currentPaletteColors.Count < 1) return;
            scheme.SetField("_" + selected + "Color", currentPaletteColors[currentPaletteIndex].UnityColor);
            SetColors(currentPaletteColors ?? new List<QuantizedColor>(), scheme, currentlevelID);
        }
        public void ToggleSelected(bool value, Toggle tgle)
        {
            if (!value) return;
            selected = tgle.gameObject.name;
        }

        public void SetColors(List<QuantizedColor> paletteColors, ColorScheme setScheme, string levelID)
        {
			toggle.GetComponent<Button>().interactable = true;
            currentlevelID = levelID;
            currentPaletteColors = paletteColors;
            scheme = setScheme;
            if (leftImg == null)
            {
                if(templateImg == null)
                {
                    var orig = Resources.FindObjectsOfTypeAll<EditColorSchemeController>().First(x => x.name == "EditColorSchemeController").transform.Find("Content").Find("ColorSchemeColorsToggleGroup");
                    templateImg = GameObject.Instantiate(orig.Find("SaberA").gameObject);
                    obsTemplateImg = GameObject.Instantiate(orig.Find("Obstacles").gameObject);
					GameObject.DontDestroyOnLoad(templateImg);
					GameObject.DontDestroyOnLoad(obsTemplateImg);
                }

                leftImg = GameObject.Instantiate(templateImg, colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                var parent = leftImg.transform.parent;
                var hover = parent.gameObject.AddComponent<HoverHint>();
                hover.text = "Left Saber/Left Lights Color";
                hover.SetField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
                var leftToggle = parent.GetComponent<Toggle>();
                leftToggle.isOn = true;
                leftToggle.onValueChanged.AddListener(b => { ToggleSelected(b, leftToggle); });
                parent.gameObject.name = "saberA";

                rightImg = GameObject.Instantiate(templateImg, colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                var parent1 = rightImg.transform.parent;
                var hover2 = parent1.gameObject.AddComponent<HoverHint>();
                hover2.text = "Right Saber/Right Lights Color";
                hover2.SetField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
                var rightToggle = parent1.GetComponent<Toggle>();
                rightToggle.onValueChanged.AddListener(b => { ToggleSelected(b, rightToggle); });
                parent1.gameObject.name = "saberB";

                obsImg = GameObject.Instantiate(templateImg, colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                var parent2 = obsImg.transform.parent;
                var hover3 = parent2.gameObject.AddComponent<HoverHint>();
                hover3.text = "Obstacle Color";
                hover3.SetField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
                var obsToggle = parent2.GetComponent<Toggle>();
                obsToggle.onValueChanged.AddListener(b => { ToggleSelected(b, obsToggle); });
                parent2.gameObject.name = "obstacles";

                leftImgIcon = parent.Find("Icon").GetComponent<ImageView>();
                rightImgIcon = parent1.Find("Icon").GetComponent<ImageView>();
                obsImgIcon = parent2.Find("Icon").GetComponent<ImageView>();

                obsImgIcon.sprite = obsTemplateImg.transform.Find("Icon").GetComponent<ImageView>().sprite;

                var layout = colors.GetComponent<HorizontalLayoutGroup>();
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
                layout.childControlHeight = false;
                layout.childControlWidth = false;
                layout.childAlignment = TextAnchor.MiddleCenter;

                layout.spacing = 2.5f;
                colors.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;


                paletteRect = AllColors.GetComponent<RectTransform>();
                paletteRect.sizeDelta = new Vector2(20, 75);
                var vert = AllColors.GetComponent<VerticalLayoutGroup>();
                vert.childAlignment = TextAnchor.UpperCenter;
                vert.spacing = 0;
                vert.childForceExpandWidth = false;
                vert.childForceExpandHeight = false;
                vert.childControlHeight = false;
                vert.childControlWidth = false;

                var group = paletteRect.gameObject.AddComponent<ToggleGroup>();

                col1 = GameObject.Instantiate(templateImg, paletteRect.transform, false);
				GameObject.Destroy(col1.transform.Find("Icon").gameObject);
                col1Img = col1.transform.Find("ColorImage").GetComponent<ImageView>();
                col1Img.sprite = leftImg.sprite;
                col1Img.material = leftImg.material;
                var col1Toggle = col1.GetComponent<Toggle>();
                col1Toggle.group = group;
                col1Toggle.onValueChanged.AddListener(value => { if (value) currentPaletteIndex = 0; });

                col2 = GameObject.Instantiate(templateImg, paletteRect.transform, false);
				GameObject.Destroy(col2.transform.Find("Icon").gameObject);
                col2Img = col2.transform.Find("ColorImage").GetComponent<ImageView>();
                col2Img.sprite = leftImg.sprite;
                col2Img.material = leftImg.material;
                var col2Toggle = col2.GetComponent<Toggle>();
                col2Toggle.group = group;
                col2Toggle.onValueChanged.AddListener(value => { if (value) currentPaletteIndex = 1; });

                col3 = GameObject.Instantiate(templateImg, paletteRect.transform, false);
				GameObject.Destroy(col3.transform.Find("Icon").gameObject);
                col3Img = col3.transform.Find("ColorImage").GetComponent<ImageView>();
                col3Img.sprite = leftImg.sprite;
                col3Img.material = leftImg.material;
                var col3Toggle = col3.GetComponent<Toggle>();
                col3Toggle.group = group;
                col3Toggle.onValueChanged.AddListener(value => { if (value) currentPaletteIndex = 2; });

                col4 = GameObject.Instantiate(templateImg, paletteRect.transform, false);
				GameObject.Destroy(col4.transform.Find("Icon").gameObject);
                col4Img = col4.transform.Find("ColorImage").GetComponent<ImageView>();
                col4Img.sprite = leftImg.sprite;
                col4Img.material = leftImg.material;
                var col4Toggle = col4.GetComponent<Toggle>();
                col4Toggle.group = group;
                col4Toggle.onValueChanged.AddListener(value => { if (value) currentPaletteIndex = 3; });

                col5 = GameObject.Instantiate(templateImg, paletteRect.transform, false);
				GameObject.Destroy(col5.transform.Find("Icon").gameObject);
                col5Img = col5.transform.Find("ColorImage").GetComponent<ImageView>();
                col5Img.sprite = leftImg.sprite;
                col5Img.material = leftImg.material;
                var col5Toggle = col5.GetComponent<Toggle>();
                col5Toggle.group = group;
                col5Toggle.onValueChanged.AddListener(value => { if (value) currentPaletteIndex = 4; });

                group.SetAllTogglesOff();
            }

            col1Img.color = currentPaletteColors[0].UnityColor;
            col2Img.color = currentPaletteColors[1].UnityColor;
            col3Img.color = currentPaletteColors[2].UnityColor;
            col4Img.color = currentPaletteColors[3].UnityColor;
            col5Img.color = currentPaletteColors[4].UnityColor;

            leftImg.color = scheme.saberAColor;
            rightImg.color = scheme.saberBColor;
            obsImg.color = scheme.obstaclesColor;

            leftImgIcon.color = UnityEngine.Color.white;
            rightImgIcon.color = UnityEngine.Color.white;
			obsImgIcon.color = UnityEngine.Color.white;
        }
    }
}
