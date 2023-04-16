using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using BeatSaberMarkupLanguage.Attributes;
using ColorThief;
using CoverColorSaber.Configuration;
using CoverColorSaber.Util;
using HMUI;
using IPA.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CoverColorSaber.Settings
{
    internal class Menu : PersistentSingleton<Menu>
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

        private static ImageView leftImg;
        private static ImageView rightImg;
        private static ImageView obsImg;
        private static ImageView leftImgIcon;
        private static ImageView rightImgIcon;
        private static ImageView obsImgIcon;
        private static ColorScheme scheme;
        private static string currentlevelID;
        private static int currentPaletteIndex;
        private static RectTransform paletteRect;
        private static GameObject templateImg;
        private static GameObject obsTemplateImg;
        private static List<QuantizedColor> currentPaletteColors = new List<QuantizedColor>();
        private static string selected = "saberA";

        private static GameObject col1;
        private static ImageView col1Img;
        private static GameObject col2;
        private static ImageView col2Img;
        private static GameObject col3;
        private static ImageView col3Img;
        private static GameObject col4;
        private static ImageView col4Img;
        private static GameObject col5;
        private static ImageView col5Img;
        
        [UIAction("SetVal")]
        public void SetVal()
        {
            //bad
            scheme.SetField("_" + selected + "Color", currentPaletteColors[currentPaletteIndex].UnityColor);
            SetColors(currentPaletteColors ?? new List<QuantizedColor>(), scheme, null, currentlevelID);
        }
        public void ToggleSelected(bool value, Toggle tgle)
        {
            if (!value) return;
            selected = tgle.gameObject.name;
        }
        public double getLuminance(UnityEngine.Color color)
        {
            double r = Math.Pow(color.r, 2.2f);
            double g = Math.Pow(color.g, 2.2f);
            double b = Math.Pow(color.b, 2.2f);
            return 0.2126 * r + 0.7151 * g + 0.0721 * b;
        }

        public double getContrastRation(UnityEngine.Color color1, UnityEngine.Color color2) {
            var l1 = getLuminance(color1);
            var l2 = getLuminance(color2);
            var cr = (l1 > l2) ? (l1 + .05) / (l2 + .05) : (l2 + .05) / (l1 + .05);
            return cr;
        }

        public UnityEngine.Color getMostReadable(UnityEngine.Color color)
        {
            var cr = new Dictionary<string, Tuple<double, UnityEngine.Color>>();
            for (int i = 0; i < PreDefColors.colors.Count; i++)
            {
                UnityEngine.Color x = PreDefColors.colors.Values.ElementAt(i) / 255f;
                string name = PreDefColors.colors.Keys.ElementAt(i);
                if (name == "")
                    continue;

                //using contrast Ration as key gave errors since i guess some colors give the same ration sometimes;
                var ret = (getContrastRation(color, x), x);
                cr.Add(name, ret.ToTuple());
            }
            var sorted = cr.OrderBy(x => x.Value.Item1).Reverse().ToDictionary(x => x.Key, x => x.Value);
            var mostReadble = sorted.First((x) => { return x.Value.Item1 > 0f; });
            var newColor = new UnityEngine.Color(mostReadble.Value.Item2.r, mostReadble.Value.Item2.g, mostReadble.Value.Item2.b, 1.0f);
            //Console.WriteLine(mostReadble.Key + ": " + ColorUtility.ToHtmlStringRGB(color) + " " + ColorUtility.ToHtmlStringRGB(newColor) + " " + mostReadble.Value.Item1);
            return newColor;
        }

        public void SetColors(List<ColorThief.QuantizedColor> paletteColors, ColorScheme setScheme, Sprite sprite, string levelID)
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
                    templateImg = Instantiate(orig.Find("SaberA").gameObject);
                    obsTemplateImg = Instantiate(orig.Find("Obstacles").gameObject);
                    DontDestroyOnLoad(templateImg);
                    DontDestroyOnLoad(obsTemplateImg);
                }

                leftImg = Instantiate(templateImg, colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                var parent = leftImg.transform.parent;
                var hover = parent.gameObject.AddComponent<HoverHint>();
                hover.text = "Left Saber/Left Lights Color";
                hover.SetField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
                var leftToggle = parent.GetComponent<Toggle>();
                leftToggle.isOn = true;
                leftToggle.onValueChanged.AddListener(b => { ToggleSelected(b, leftToggle); });
                parent.gameObject.name = "saberA";

                rightImg = Instantiate(templateImg, colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                var parent1 = rightImg.transform.parent;
                var hover2 = parent1.gameObject.AddComponent<HoverHint>();
                hover2.text = "Right Saber/Right Lights Color";
                hover2.SetField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
                var rightToggle = parent1.GetComponent<Toggle>();
                rightToggle.onValueChanged.AddListener(b => { ToggleSelected(b, rightToggle); });
                parent1.gameObject.name = "saberB";

                obsImg = Instantiate(templateImg, colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
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

                //Destroy(Use);
                var layout = colors.GetComponent<HorizontalLayoutGroup>();
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
                layout.childControlHeight = false;
                layout.childControlWidth = false;
                layout.childAlignment = TextAnchor.MiddleCenter;

                //Destroy(Colors.GetComponent<LayoutElement>());
                //Destroy(Colors.GetComponent<ContentSizeFitter>());
                //layout.padding = new RectOffset(1, 1, 5, -1);
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
                
                col1 = Instantiate(templateImg, paletteRect.transform, false);
                Destroy(col1.transform.Find("Icon").gameObject);
                col1Img = col1.transform.Find("ColorImage").GetComponent<ImageView>();
                col1Img.sprite = leftImg.sprite;
                col1Img.material = leftImg.material;
                var col1Toggle = col1.GetComponent<Toggle>();
                col1Toggle.group = group;
                col1Toggle.onValueChanged.AddListener(value => { if (value) currentPaletteIndex = 0; });

                col2 = Instantiate(templateImg, paletteRect.transform, false);
                Destroy(col2.transform.Find("Icon").gameObject);
                col2Img = col2.transform.Find("ColorImage").GetComponent<ImageView>();
                col2Img.sprite = leftImg.sprite;
                col2Img.material = leftImg.material;
                var col2Toggle = col2.GetComponent<Toggle>();
                col2Toggle.group = group;
                col2Toggle.onValueChanged.AddListener(value => { if (value) currentPaletteIndex = 1; });

                col3 = Instantiate(templateImg, paletteRect.transform, false);
                Destroy(col3.transform.Find("Icon").gameObject);
                col3Img = col3.transform.Find("ColorImage").GetComponent<ImageView>();
                col3Img.sprite = leftImg.sprite;
                col3Img.material = leftImg.material;
                var col3Toggle = col3.GetComponent<Toggle>();
                col3Toggle.group = group;
                col3Toggle.onValueChanged.AddListener(value => { if (value) currentPaletteIndex = 2; });

                col4 = Instantiate(templateImg, paletteRect.transform, false);
                Destroy(col4.transform.Find("Icon").gameObject);
                col4Img = col4.transform.Find("ColorImage").GetComponent<ImageView>();
                col4Img.sprite = leftImg.sprite;
                col4Img.material = leftImg.material;
                var col4Toggle = col4.GetComponent<Toggle>();
                col4Toggle.group = group;
                col4Toggle.onValueChanged.AddListener(value => { if (value) currentPaletteIndex = 3; });

                col5 = Instantiate(templateImg, paletteRect.transform, false);
                Destroy(col5.transform.Find("Icon").gameObject);
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

            leftImgIcon.color = getMostReadable(scheme.saberAColor);
            rightImgIcon.color = getMostReadable(scheme.saberBColor);
            obsImgIcon.color = getMostReadable(scheme.obstaclesColor); 
        }
    }
}
