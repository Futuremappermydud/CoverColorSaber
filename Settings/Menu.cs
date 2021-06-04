using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using CoverColorSaber.Configuration;
using HMUI;
using IPA.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CoverColorSaber.Settings
{
    //super monkas class
    internal class Menu : PersistentSingleton<Menu>
    {
        [UIComponent("SongNameText")]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private TextMeshProUGUI songNameText = null;

        [UIObject("Song")]
#pragma warning disable 414
        private GameObject songObj = null;
#pragma warning restore 414

        private string songName;
        public string SongName
        {
            
            get => songName;
            set
            {
                songName = value;
                songNameText.text = songName;
                //Plugin.Log.Info("Settings Song Name to " + value);
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
        private Texture2D currentTex;
        private RectTransform paletteRect;
        private GameObject templateImg;
        private GameObject obsTemplateImg;
        private List<ColorThief.QuantizedColor> currentPaletteColors = new List<ColorThief.QuantizedColor>();
        private string selected = "saberA";

        //Sorry
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

        public void SetVal(int paletteIndex)
        {
            //this feels really monkas
            scheme.SetField("_" + selected + "Color", currentPaletteColors[paletteIndex].UnityColor);
            SetColors(currentPaletteColors ?? new List<ColorThief.QuantizedColor>(), scheme, currentTex, currentlevelID);
        }
        Color InvertColor(Color color) {
            return new Color(1.0f-color.r, 1.0f-color.g, 1.0f-color.b);
        }
        public void ToggleSelected(bool value, Toggle tgle)
        {
            if (!value) return;
            selected = tgle.gameObject.name;
        }

        private static Button CreateBaseButton(string name, RectTransform parent, string buttonTemplate)
        {
            var btn = UnityEngine.Object.Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == buttonTemplate)), parent, false);
            btn.name = name;
            btn.interactable = true;
            return btn;
        }

        private static Button CreateUIButton(string name, RectTransform parent, string buttonTemplate, Vector2 anchoredPosition, Vector2 sizeDelta, UnityAction onClick = null, string buttonText = "BUTTON")
        {
            //Logger.Debug("CreateUIButton({0}, {1}, {2}, {3}, {4}", name, parent, buttonTemplate, anchoredPosition, sizeDelta);
            var btn = CreateBaseButton(name, parent, buttonTemplate);
            btn.gameObject.SetActive(true);

            var localizer = btn.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>();
            if (localizer != null)
            {
                GameObject.Destroy(localizer);
            }
            var externalComponents = btn.gameObject.AddComponent<BeatSaberMarkupLanguage.Components.ExternalComponents>();
            var textMesh = btn.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.richText = true;
            externalComponents.components.Add(textMesh);

            var contentTransform = btn.transform.Find("Content").GetComponent<LayoutElement>();
            if (contentTransform != null)
            {
                GameObject.Destroy(contentTransform);
            }
            var buttonSizeFitter = btn.gameObject.AddComponent<ContentSizeFitter>();
            buttonSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            buttonSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            var stackLayoutGroup = btn.GetComponentInChildren<LayoutGroup>();
            if (stackLayoutGroup != null)
            {
                externalComponents.components.Add(stackLayoutGroup);
            }

            btn.onClick.RemoveAllListeners();
            if (onClick != null)
            {
                btn.onClick.AddListener(onClick);
            }

            var transform1 = btn.transform;
            ((RectTransform) transform1).anchorMin = new Vector2(0.5f, 0.5f);
            ((RectTransform) transform1).anchorMax = new Vector2(0.5f, 0.5f);
            ((RectTransform) transform1).anchoredPosition = anchoredPosition;
            ((RectTransform) transform1).sizeDelta = sizeDelta;

            btn.SetButtonText(buttonText);

            return btn;
        }

        public void SetColors(List<ColorThief.QuantizedColor> paletteColors, ColorScheme setScheme, Texture2D coverImg, string levelID)
        {
            currentlevelID = levelID;
            currentPaletteColors = paletteColors;
            currentTex = coverImg;
            this.scheme = setScheme;
            if (leftImg == null)
            {
                /*
                var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.name = "CoverImage_Track";
                planeRend = plane.GetComponent<Renderer>();
                planeRend.material = new Material(Shader.Find("Unlit/Texture"));
                plane.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                plane.transform.position = new Vector3(0f, 0.01f, 1.2f);
                plane.transform.Rotate(new Vector3(0f, 180f, 0f));
                plane.transform.SetParent(songObj.transform, true);

                var pointer = Resources.FindObjectsOfTypeAll<VRPointer>().FirstOrDefault();

                tracker = new GameObject("PointerTracker").AddComponent<PointerTracker>();
                tracker.Track(pointer, "CoverImage_Track");
                tracker.HitTexCoord += RayCastCoord;*/

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

                //Palette
                var paletteObj = new GameObject("Palette Object");
                var parent3 = colors.transform.parent.parent;
                paletteObj.transform.SetParent(parent3, false);
                paletteObj.transform.localPosition = new Vector3(-36.67f, -12.50f, 0);

                paletteRect = paletteObj.AddComponent<RectTransform>();
                paletteRect.sizeDelta = new Vector2(20, 75);
                var vert = paletteObj.AddComponent<VerticalLayoutGroup>();
                vert.childAlignment = TextAnchor.UpperCenter;
                vert.spacing = 0;
                vert.childForceExpandWidth = false;
                vert.childForceExpandHeight = false;
                vert.childControlHeight = false;
                vert.childControlWidth = false;

                var group = paletteRect.gameObject.AddComponent<ToggleGroup>();

                col1 = Instantiate(templateImg, colors.transform, false);
                Destroy(col1.transform.Find("Icon").gameObject);
                col1.transform.SetParent(paletteRect.transform, false);
                col1Img = col1.transform.Find("ColorImage").GetComponent<ImageView>();
                //image.color = currentPaletteIndex[0].UnityColor;
                col1Img.sprite = leftImg.sprite;
                col1Img.material = leftImg.material;
                var col1Toggle = col1.GetComponent<Toggle>();
                col1Toggle.group = group;
                col1Toggle.onValueChanged.AddListener(new UnityAction<bool>( value => { if (value) currentPaletteIndex = 0; } ));

                col2 = Instantiate(templateImg, colors.transform, false);
                Destroy(col2.transform.Find("Icon").gameObject);
                col2.transform.SetParent(paletteRect.transform, false);
                col2Img = col2.transform.Find("ColorImage").GetComponent<ImageView>();
                //image.color = currentPaletteIndex[1].UnityColor;
                col2Img.sprite = leftImg.sprite;
                col2Img.material = leftImg.material;
                var col2Toggle = col2.GetComponent<Toggle>();
                col2Toggle.group = group;
                col2Toggle.onValueChanged.AddListener(new UnityAction<bool>(value => { if (value) currentPaletteIndex = 1; }));

                col3 = Instantiate(templateImg, colors.transform, false);
                Destroy(col3.transform.Find("Icon").gameObject);
                col3.transform.SetParent(paletteRect.transform, false);
                col3Img = col3.transform.Find("ColorImage").GetComponent<ImageView>();
                //image.color = currentPaletteIndex[2].UnityColor;
                col3Img.sprite = leftImg.sprite;
                col3Img.material = leftImg.material;
                var col3Toggle = col3.GetComponent<Toggle>();
                col3Toggle.group = group;
                col3Toggle.onValueChanged.AddListener(new UnityAction<bool>(value => { if (value) currentPaletteIndex = 2; }));

                col4 = Instantiate(templateImg, colors.transform, false);
                Destroy(col4.transform.Find("Icon").gameObject);
                col4.transform.SetParent(paletteRect.transform, false);
                col4Img = col4.transform.Find("ColorImage").GetComponent<ImageView>();
                //col4img.color = currentPaletteIndex[3].UnityColor;
                col4Img.sprite = leftImg.sprite;
                col4Img.material = leftImg.material;
                var col4Toggle = col4.GetComponent<Toggle>();
                col4Toggle.group = group;
                col4Toggle.onValueChanged.AddListener(new UnityAction<bool>(value => { if (value) currentPaletteIndex = 3; }));

                col5 = Instantiate(templateImg, colors.transform, false);
                Destroy(col5.transform.Find("Icon").gameObject);
                col5.transform.SetParent(paletteRect.transform, false);
                col5Img = col5.transform.Find("ColorImage").GetComponent<ImageView>();
                //col5img.color = currentPaletteIndex[4].UnityColor;
                col5Img.sprite = leftImg.sprite;
                col5Img.material = leftImg.material;
                var col5Toggle = col5.GetComponent<Toggle>();
                col5Toggle.group = group;
                col5Toggle.onValueChanged.AddListener(new UnityAction<bool>(value => { if (value) currentPaletteIndex = 4; }));

                group.SetAllTogglesOff();

                var setButton = CreateUIButton("sortBy", (RectTransform)parent3, "PracticeButton", new Vector2(-25f, 0f), new Vector2(10, 0.5f), () =>
                {
                    SetVal(currentPaletteIndex);
                }, "Set");
                setButton.GetButtonText().GetComponent<RectTransform>().sizeDelta = new Vector2(10, 0.5f);
                setButton.SetButtonTextSize(5f);
                setButton.ToggleWordWrapping(false);
            }

            col1Img.color = currentPaletteColors[0].UnityColor;
            col2Img.color = currentPaletteColors[1].UnityColor;
            col3Img.color = currentPaletteColors[2].UnityColor;
            col4Img.color = currentPaletteColors[3].UnityColor;
            col5Img.color = currentPaletteColors[4].UnityColor;

            leftImg.color = setScheme.saberAColor;
            rightImg.color = setScheme.saberBColor;
            obsImg.color = setScheme.obstaclesColor;

            leftImgIcon.color = InvertColor(setScheme.saberAColor);
            rightImgIcon.color = InvertColor(setScheme.saberBColor);
            obsImgIcon.color = InvertColor(setScheme.obstaclesColor); 
        }
    }
}
