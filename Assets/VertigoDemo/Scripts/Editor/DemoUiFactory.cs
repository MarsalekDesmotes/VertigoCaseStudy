using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.Editor
{
    internal static class DemoUiFactory
    {
        public static TMP_FontAsset Font { get; set; }

        public static RectTransform Rect(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 size)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            return rect;
        }

        public static Image Image(
            string name,
            Transform parent,
            Sprite sprite,
            Color color,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 size,
            bool raycast = false,
            bool preserveAspect = false)
        {
            RectTransform rect = Rect(name, parent, anchorMin, anchorMax, pivot, anchoredPosition, size);
            Image image = rect.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = raycast;
            image.maskable = false;
            image.preserveAspect = preserveAspect;
            if (sprite != null && sprite.border.sqrMagnitude > 0f)
            {
                image.type = UnityEngine.UI.Image.Type.Sliced;
            }
            return image;
        }

        public static TMP_Text Text(
            string name,
            Transform parent,
            string content,
            float fontSize,
            Color color,
            TextAlignmentOptions alignment,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 size,
            FontStyles style = FontStyles.Normal)
        {
            RectTransform rect = Rect(name, parent, anchorMin, anchorMax, pivot, anchoredPosition, size);
            TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.font = Font;
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.fontStyle = style;
            text.enableWordWrapping = true;
            text.raycastTarget = false;
            text.maskable = false;
            return text;
        }

        public static Button Button(
            string name,
            Transform parent,
            Sprite sprite,
            Color color,
            string label,
            Vector2 anchor,
            Vector2 position,
            Vector2 size)
        {
            Image image = Image(name, parent, sprite, color, anchor, anchor, new Vector2(0.5f, 0.5f), position, size, true);
            image.maskable = true;
            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
            colors.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1f);
            colors.disabledColor = new Color(0.42f, 0.42f, 0.42f, 0.7f);
            button.colors = colors;

            Text(
                "ui_text_" + name.Substring("ui_button_".Length) + "_value",
                image.transform,
                label,
                28f,
                Color.white,
                TextAlignmentOptions.Center,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero,
                FontStyles.Bold);
            return button;
        }

        public static void Stretch(RectTransform rect, float left, float bottom, float right, float top)
        {
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }
    }
}
