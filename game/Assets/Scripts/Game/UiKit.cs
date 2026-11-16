using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Saga.Game
{
    // Small helpers for building UGUI in code, shared by the menus and the HUD.
    public static class UiKit
    {
        public static readonly Color Panel = new Color(0.04f, 0.06f, 0.1f, 0.82f);
        public static readonly Color TextColour = new Color(0.86f, 0.9f, 0.96f);
        public static readonly Color Muted = new Color(0.55f, 0.6f, 0.68f);
        public static readonly Color Accent = new Color(0.45f, 0.72f, 1f);
        static Font font;
        public static Font Font => font != null ? font : (font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        public static Canvas Canvas(string name, Camera cam, int order = 0)
        {
            if (EventSystem.current == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var c = go.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceCamera;
            c.worldCamera = cam;
            c.planeDistance = cam.nearClipPlane + 5f;
            c.sortingOrder = order;
            var s = go.GetComponent<CanvasScaler>();
            s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            s.referenceResolution = new Vector2(1920, 1080);
            s.matchWidthOrHeight = 0.5f;
            go.AddComponent<AspectFit>();
            return c;
        }

        // Layouts are made for 16:9. Wider screens keep that size; narrower ones (towards 1:1) scale the whole UI
        // down to fit the width instead of letting panels overlap.
        public static float MatchFor(float aspect) => aspect >= 16f / 9f ? 0.5f : 0f;

        public static RectTransform Rect(Transform parent, string name, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var r = (RectTransform)go.transform;
            r.SetParent(parent, false);
            r.anchorMin = r.anchorMax = anchor;
            r.pivot = anchor;
            r.anchoredPosition = pos;
            r.sizeDelta = size;
            return r;
        }

        public static Image Box(Transform parent, string name, Vector2 anchor, Vector2 pos, Vector2 size, Color colour)
        {
            var img = Rect(parent, name, anchor, pos, size).gameObject.AddComponent<Image>();
            img.color = colour;
            return img;
        }

        public static Text Label(Transform parent, string name, int size, TextAnchor align, Vector2 anchor, Vector2 pos, Vector2 box, string text = "")
        {
            var t = Rect(parent, name, anchor, pos, box).gameObject.AddComponent<Text>();
            t.font = Font; t.fontSize = size; t.alignment = align; t.color = TextColour; t.supportRichText = true;
            t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            t.text = text;
            return t;
        }

        // A clickable panel with a centred label; returns the label so callers can change its text.
        public static Text Button(Transform parent, string label, Vector2 anchor, Vector2 pos, Vector2 size, Action onClick, int fontSize = 24)
        {
            var bg = Box(parent, "button " + label, anchor, pos, size, new Color(0.12f, 0.16f, 0.24f, 0.95f));
            var outline = bg.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.3f, 0.45f, 0.65f, 0.6f);
            var b = bg.gameObject.AddComponent<Button>();
            var colours = b.colors;
            colours.highlightedColor = new Color(1.35f, 1.35f, 1.5f);
            colours.pressedColor = new Color(0.8f, 0.8f, 0.9f);
            colours.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);
            b.colors = colours;
            b.onClick.AddListener(() => onClick());
            return Label(bg.transform, "label", fontSize, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), Vector2.zero, size, label);
        }
    }
}

namespace Saga.Game
{
    // Keeps a canvas's scaling right as the window changes shape (see UiKit.MatchFor).
    public class AspectFit : MonoBehaviour
    {
        CanvasScaler scaler;
        void Awake() => scaler = GetComponent<CanvasScaler>();
        void Update()
        {
            var cam = GetComponent<Canvas>().worldCamera;
            float aspect = cam != null ? cam.aspect : (float)Screen.width / Mathf.Max(1, Screen.height);
            scaler.matchWidthOrHeight = UiKit.MatchFor(aspect);
        }
    }
}
