using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using IPA.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoverColorSaber
{
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
        public bool schemeEnabled = true;

        [UIObject("Colors")] private GameObject Colors = null;

        private ImageView leftImg = null;
        private ImageView rightImg = null;
        private ImageView obsImg = null;
        private ImageView leftImgIcon = null;
        private ImageView rightImgIcon = null;
        private ImageView obsImgIcon = null;
        Color InvertColor(Color color) {
            return new Color(1.0f-color.r, 1.0f-color.g, 1.0f-color.b);
        }
        public void SetColors(ColorScheme scheme)
        {
            Plugin.Log.Info("Setting up colors");
            if (leftImg == null)
            {
                GameObject Use = Instantiate(Resources.FindObjectsOfTypeAll<EditColorSchemeController>().First((EditColorSchemeController x) => { return x.name == "EditColorSchemeController"; }).transform.Find("Content").Find("ColorSchemeColorsToggleGroup").Find("SaberA").gameObject);
                Destroy(Use.GetComponent<Toggle>());
                Destroy(Use.transform.Find("Selection").gameObject);
                Destroy(Use.transform.Find("Highlight").gameObject);
                leftImg = Instantiate(Use.gameObject, Colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                HoverHint hover = leftImg.transform.parent.gameObject.AddComponent<HoverHint>();
                hover.text = "Left Saber/Left Lights Color";
                hover.SetField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());  

                rightImg = Instantiate(Use.gameObject, Colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                HoverHint hover2 = rightImg.transform.parent.gameObject.AddComponent<HoverHint>();
                hover2.text = "Right Saber/Right Lights Color";
                hover2.SetField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());

                obsImg = Instantiate(Use.gameObject, Colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                HoverHint hover3 = obsImg.transform.parent.gameObject.AddComponent<HoverHint>();
                hover3.text = "Obstacle Color";
                hover3.SetField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());

                leftImgIcon = leftImg.transform.parent.Find("Icon").GetComponent<ImageView>();
                rightImgIcon = rightImg.transform.parent.Find("Icon").GetComponent<ImageView>();
                obsImgIcon = obsImg.transform.parent.Find("Icon").GetComponent<ImageView>();

                Destroy(Use);
                HorizontalLayoutGroup layout = Colors.GetComponent<HorizontalLayoutGroup>();
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
                layout.childControlHeight = false;
                layout.childControlWidth = false;
                layout.childAlignment = TextAnchor.MiddleCenter;

                Destroy(Colors.GetComponent<LayoutElement>());
                Destroy(Colors.GetComponent<ContentSizeFitter>());
                //layout.padding = new RectOffset(1, 1, 5, -1);
                layout.spacing = 2.5f;
                Colors.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            leftImg.color = scheme.saberAColor;
            rightImg.color = scheme.saberBColor;
            obsImg.color = scheme.obstaclesColor;

            leftImgIcon.color = InvertColor(scheme.saberAColor);
            rightImgIcon.color = InvertColor(scheme.saberBColor);
            obsImgIcon.color = InvertColor(scheme.obstaclesColor);
        }
    }
}
