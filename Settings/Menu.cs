using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using IPA.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRUIControls;

namespace CoverColorSaber
{
    //super monkas class
    internal class Menu : PersistentSingleton<Menu>
    {
        [UIComponent("SongNameText")]
        private TextMeshProUGUI songNameText = null;

        [UIObject("Song")]
        private GameObject songObj = null;

        private string _songName;
        public string songName
        {
            get { return _songName; }
            set
            {
                _songName = value;
                songNameText.text = _songName;
                //Plugin.Log.Info("Settings Song Name to " + value);
            }
        }
        [UIValue("enabled")]
        public bool schemeEnabled
        { 
            get 
            {
                return PluginConfig.Instance.enabled;
            } 
            set
            {
                PluginConfig.Instance.enabled = value;
            }
        }

        [UIObject("Colors")] private GameObject Colors = null;

        public ViewController inst = null;
        private ImageView leftImg = null;
        private ImageView rightImg = null;
        private ImageView obsImg = null;
        private ImageView leftImgIcon = null;
        private ImageView rightImgIcon = null;
        private ImageView obsImgIcon = null;
        private Renderer planeRend;
        private PointerTracker tracker;
        private ColorScheme scheme;
        private string currentlevelID;
        private int currentQuantIndex;
        private Texture2D currentTex;
        private RectTransform paletteRect;
        private GameObject templateImg;
        private GameObject obsTemplateImg;
        private List<ColorThief.QuantizedColor> currentQuantColors = new List<ColorThief.QuantizedColor>();
        private string selected = "saberA";

        //Sorry
        private GameObject col1;
        private ImageView col1img;
        private GameObject col2;
        private ImageView col2img;
        private GameObject col3;
        private ImageView col3img;
        private GameObject col4;
        private ImageView col4img;
        private GameObject col5;
        private ImageView col5img;

        public void SetVal(int quantIndex)
        {
            //this feels really monkas
            scheme.SetField("_" + selected + "Color", currentQuantColors[quantIndex].UnityColor);
            SetColors(currentQuantColors ?? new List<ColorThief.QuantizedColor>(), scheme, currentTex, currentlevelID);
        }
        Color InvertColor(Color color) {
            return new Color(1.0f-color.r, 1.0f-color.g, 1.0f-color.b);
        }
        public void ToggleSelected(bool value, Toggle tgle)
        {
            if (!value) return;
            selected = tgle.gameObject.name;
        }

        public static Button CreateBaseButton(string name, RectTransform parent, string buttonTemplate)
        {
            Button btn = UnityEngine.Object.Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == buttonTemplate)), parent, false);
            btn.name = name;
            btn.interactable = true;
            return btn;
        }

        public static Button CreateUIButton(string name, RectTransform parent, string buttonTemplate, Vector2 anchoredPosition, Vector2 sizeDelta, UnityAction onClick = null, string buttonText = "BUTTON")
        {
            //Logger.Debug("CreateUIButton({0}, {1}, {2}, {3}, {4}", name, parent, buttonTemplate, anchoredPosition, sizeDelta);
            Button btn = CreateBaseButton(name, parent, buttonTemplate);
            btn.gameObject.SetActive(true);

            Polyglot.LocalizedTextMeshProUGUI localizer = btn.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>();
            if (localizer != null)
            {
                GameObject.Destroy(localizer);
            }
            BeatSaberMarkupLanguage.Components.ExternalComponents externalComponents = btn.gameObject.AddComponent<BeatSaberMarkupLanguage.Components.ExternalComponents>();
            TextMeshProUGUI textMesh = btn.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.richText = true;
            externalComponents.components.Add(textMesh);

            var contentTransform = btn.transform.Find("Content").GetComponent<LayoutElement>();
            if (contentTransform != null)
            {
                GameObject.Destroy(contentTransform);
            }

            ContentSizeFitter buttonSizeFitter = btn.gameObject.AddComponent<ContentSizeFitter>();
            buttonSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            buttonSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            LayoutGroup stackLayoutGroup = btn.GetComponentInChildren<LayoutGroup>();
            if (stackLayoutGroup != null)
            {
                externalComponents.components.Add(stackLayoutGroup);
            }

            btn.onClick.RemoveAllListeners();
            if (onClick != null)
            {
                btn.onClick.AddListener(onClick);
            }

            (btn.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
            (btn.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
            (btn.transform as RectTransform).anchoredPosition = anchoredPosition;
            (btn.transform as RectTransform).sizeDelta = sizeDelta;

            btn.SetButtonText(buttonText);

            return btn;
        }

        public void SetColors(List<ColorThief.QuantizedColor> quantColors, ColorScheme scheme, Texture2D coverImg, string levelID)
        {
            currentlevelID = levelID;
            currentQuantColors = quantColors;
            currentTex = coverImg;
            this.scheme = scheme;
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
                    var Orig = Resources.FindObjectsOfTypeAll<EditColorSchemeController>().First((EditColorSchemeController x) => { return x.name == "EditColorSchemeController"; }).transform.Find("Content").Find("ColorSchemeColorsToggleGroup");
                    templateImg = Instantiate(Orig.Find("SaberA").gameObject);
                    obsTemplateImg = Instantiate(Orig.Find("Obstacles").gameObject);
                    DontDestroyOnLoad(templateImg);
                }

                leftImg = Instantiate(templateImg, Colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                var hover = leftImg.transform.parent.gameObject.AddComponent<HoverHint>();
                hover.text = "Left Saber/Left Lights Color";
                hover.SetField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
                Toggle leftTgle = leftImg.transform.parent.GetComponent<Toggle>();
                leftTgle.isOn = true;
                leftTgle.onValueChanged.AddListener((bool b) => { ToggleSelected(b, leftTgle); });
                leftImg.transform.parent.gameObject.name = "saberA";

                rightImg = Instantiate(templateImg, Colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                var hover2 = rightImg.transform.parent.gameObject.AddComponent<HoverHint>();
                hover2.text = "Right Saber/Right Lights Color";
                hover2.SetField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
                Toggle rightTgle = rightImg.transform.parent.GetComponent<Toggle>();
                rightTgle.onValueChanged.AddListener((bool b) => { ToggleSelected(b, rightTgle); });
                rightImg.transform.parent.gameObject.name = "saberB";

                obsImg = Instantiate(templateImg, Colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                var hover3 = obsImg.transform.parent.gameObject.AddComponent<HoverHint>();
                hover3.text = "Obstacle Color";
                hover3.SetField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
                Toggle obsTgle = obsImg.transform.parent.GetComponent<Toggle>();
                obsTgle.onValueChanged.AddListener((bool b) => { ToggleSelected(b, obsTgle); });
                obsImg.transform.parent.gameObject.name = "obstacles";

                leftImgIcon = leftImg.transform.parent.Find("Icon").GetComponent<ImageView>();
                rightImgIcon = rightImg.transform.parent.Find("Icon").GetComponent<ImageView>();
                obsImgIcon = obsImg.transform.parent.Find("Icon").GetComponent<ImageView>();

                obsImgIcon.sprite = obsTemplateImg.transform.Find("Icon").GetComponent<ImageView>().sprite;

                //Destroy(Use);
                HorizontalLayoutGroup layout = Colors.GetComponent<HorizontalLayoutGroup>();
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
                layout.childControlHeight = false;
                layout.childControlWidth = false;
                layout.childAlignment = TextAnchor.MiddleCenter;

                //Destroy(Colors.GetComponent<LayoutElement>());
                //Destroy(Colors.GetComponent<ContentSizeFitter>());
                //layout.padding = new RectOffset(1, 1, 5, -1);
                layout.spacing = 2.5f;
                Colors.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                //Palette
                GameObject paletteObj = new GameObject("Palette Object");
                paletteObj.transform.SetParent(Colors.transform.parent.parent, false);
                paletteObj.transform.localPosition = new Vector3(-36.67f, -12.50f, 0);

                paletteRect = paletteObj.AddComponent<RectTransform>();
                paletteRect.sizeDelta = new Vector2(20, 75);
                VerticalLayoutGroup vert = paletteObj.AddComponent<VerticalLayoutGroup>();
                vert.childAlignment = TextAnchor.UpperCenter;
                vert.spacing = 0;
                vert.childForceExpandWidth = false;
                vert.childForceExpandHeight = false;
                vert.childControlHeight = false;
                vert.childControlWidth = false;

                ToggleGroup group = paletteRect.gameObject.AddComponent<ToggleGroup>();

                col1 = Instantiate(templateImg, Colors.transform, false);
                Destroy(col1.transform.Find("Icon").gameObject);
                col1.transform.SetParent(paletteRect.transform, false);
                col1img = col1.transform.Find("ColorImage").GetComponent<ImageView>();
                //image.color = currentQuantColors[0].UnityColor;
                col1img.sprite = leftImg.sprite;
                col1img.material = leftImg.material;
                var col1Toggle = col1.GetComponent<Toggle>();
                col1Toggle.group = group;
                col1Toggle.onValueChanged.AddListener(new UnityAction<bool>( (bool value) => { if (value) currentQuantIndex = 0; }));

                col2 = Instantiate(templateImg, Colors.transform, false);
                Destroy(col2.transform.Find("Icon").gameObject);
                col2.transform.SetParent(paletteRect.transform, false);
                col2img = col2.transform.Find("ColorImage").GetComponent<ImageView>();
                //image.color = currentQuantColors[1].UnityColor;
                col2img.sprite = leftImg.sprite;
                col2img.material = leftImg.material;
                var col2Toggle = col2.GetComponent<Toggle>();
                col2Toggle.group = group;
                col2Toggle.onValueChanged.AddListener(new UnityAction<bool>((bool value) => { if (value) currentQuantIndex = 1; }));

                col3 = Instantiate(templateImg, Colors.transform, false);
                Destroy(col3.transform.Find("Icon").gameObject);
                col3.transform.SetParent(paletteRect.transform, false);
                col3img = col3.transform.Find("ColorImage").GetComponent<ImageView>();
                //image.color = currentQuantColors[2].UnityColor;
                col3img.sprite = leftImg.sprite;
                col3img.material = leftImg.material;
                var col3Toggle = col3.GetComponent<Toggle>();
                col3Toggle.group = group;
                col3Toggle.onValueChanged.AddListener(new UnityAction<bool>((bool value) => { if (value) currentQuantIndex = 2; }));

                col4 = Instantiate(templateImg, Colors.transform, false);
                Destroy(col4.transform.Find("Icon").gameObject);
                col4.transform.SetParent(paletteRect.transform, false);
                col4img = col4.transform.Find("ColorImage").GetComponent<ImageView>();
                //col4img.color = currentQuantColors[3].UnityColor;
                col4img.sprite = leftImg.sprite;
                col4img.material = leftImg.material;
                var col4Toggle = col4.GetComponent<Toggle>();
                col4Toggle.group = group;
                col4Toggle.onValueChanged.AddListener(new UnityAction<bool>((bool value) => { if (value) currentQuantIndex = 3; }));

                col5 = Instantiate(templateImg, Colors.transform, false);
                Destroy(col5.transform.Find("Icon").gameObject);
                col5.transform.SetParent(paletteRect.transform, false);
                col5img = col5.transform.Find("ColorImage").GetComponent<ImageView>();
                //col5img.color = currentQuantColors[4].UnityColor;
                col5img.sprite = leftImg.sprite;
                col5img.material = leftImg.material;
                var col5Toggle = col5.GetComponent<Toggle>();
                col5Toggle.group = group;
                col5Toggle.onValueChanged.AddListener(new UnityAction<bool>((bool value) => { if (value) currentQuantIndex = 4; }));

                group.SetAllTogglesOff();

                var setButton = CreateUIButton("sortBy", (RectTransform)Colors.transform.parent.parent, "PracticeButton", new Vector2(-25f, 0f), new Vector2(10, 0.5f), () =>
                {
                    SetVal(currentQuantIndex);
                }, "Set");
                setButton.GetButtonText().GetComponent<RectTransform>().sizeDelta = new Vector2(10, 0.5f);
                setButton.SetButtonTextSize(5f);
                setButton.ToggleWordWrapping(false);
            }

            col1img.color = currentQuantColors[0].UnityColor;
            col2img.color = currentQuantColors[1].UnityColor;
            col3img.color = currentQuantColors[2].UnityColor;
            col4img.color = currentQuantColors[3].UnityColor;
            col5img.color = currentQuantColors[4].UnityColor;

            leftImg.color = scheme.saberAColor;
            rightImg.color = scheme.saberBColor;
            obsImg.color = scheme.obstaclesColor;

            leftImgIcon.color = InvertColor(scheme.saberAColor);
            rightImgIcon.color = InvertColor(scheme.saberBColor);
            obsImgIcon.color = InvertColor(scheme.obstaclesColor); 
        }
    }
}
