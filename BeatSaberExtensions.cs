using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace CoverColorSaber
{
    public static class BeatSaberUIExtensions
    {
        #region Button Extensions
        public static void SetButtonText(this Button button, string text)
        {
            var textMesh = button.GetComponentInChildren<HMUI.CurvedTextMeshPro>();
            if (textMesh != null)
            {
                textMesh.SetText(text);
            }
        }

        public static HMUI.CurvedTextMeshPro GetButtonText(this Button button)
        {
            var textMesh = button.GetComponentInChildren<HMUI.CurvedTextMeshPro>();
            return textMesh;
        }

        public static void SetButtonTextSize(this Button button, float fontSize)
        {
            var txtMesh = button.GetComponentInChildren<HMUI.CurvedTextMeshPro>();
            if (txtMesh != null)
            {
                txtMesh.fontSize = fontSize;
            }
        }

        public static void ToggleWordWrapping(this Button button, bool enableWordWrapping)
        {
            var txtMesh = button.GetComponentInChildren<HMUI.CurvedTextMeshPro>();
            if (txtMesh != null)
            {
                txtMesh.enableWordWrapping = enableWordWrapping;
            }
        }

        public static void SetButtonBackgroundActive(this Button parent, bool active)
        {
            var img = parent.GetComponentsInChildren<HMUI.ImageView>().Last(x => x.name == "BG");
            if (img != null)
            {
                img.gameObject.SetActive(active);
            }
        }

        public static void SetButtonUnderlineColor(this Button parent, Color color)
        {
            var img = parent.GetComponentsInChildren<HMUI.ImageView>().FirstOrDefault(x => x.name == "Underline");
            if (img != null)
            {
                img.color = color;
            }
        }

        public static void SetButtonBorder(this Button button, Color color)
        {
            var img = button.GetComponentsInChildren<HMUI.ImageView>().FirstOrDefault(x => x.name == "Border");
            if (img != null)
            {
                img.color0 = color;
                img.color1 = color;
                img.color = color;
                img.fillMethod = Image.FillMethod.Horizontal;
                img.SetAllDirty();
            }
        }
        #endregion
    }
}