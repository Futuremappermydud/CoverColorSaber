using System;
using System.Collections.Generic;
using System.Linq;

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
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
                rightImg = Instantiate(Use.gameObject, Colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                obsImg = Instantiate(Use.gameObject, Colors.transform, false).transform.Find("ColorImage").GetComponent<ImageView>();
                Destroy(Use);
                HorizontalLayoutGroup layout = Colors.GetComponent<HorizontalLayoutGroup>();
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
                layout.childAlignment = TextAnchor.MiddleCenter;
                //layout.padding = new RectOffset(1, 1, 5, -1);
                layout.spacing = 0.2f;
                Colors.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            leftImg.color = scheme.saberAColor;
            rightImg.color = scheme.saberBColor;
            obsImg.color = scheme.obstaclesColor;
        }
    }
}
