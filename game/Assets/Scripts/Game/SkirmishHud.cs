using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saga.Sim;
using UnityEngine;
using UnityEngine.UI;

namespace Saga.Game
{
    // On-screen information for the skirmish, built in code: fleet counts and clock, a controls card,
    // the selected ships (icons with health), and the end-of-battle banner.
    public class SkirmishHud
    {
        const int MaxCells = 24;
        static readonly Color Panel = new Color(0.04f, 0.06f, 0.1f, 0.72f);
        static readonly Color TextColour = new Color(0.86f, 0.9f, 0.96f);

        readonly SkirmishController game;
        readonly ShipLibrary lib;
        readonly Camera cam;
        readonly Font font;
        readonly Text status, banner, detail, help;
        readonly RectTransform selectionPanel, helpPanel;
        readonly List<(GameObject cell, Image icon, Image hp, Image sh)> cells = new List<(GameObject, Image, Image, Image)>();
        float helpUntil;

        public SkirmishHud(SkirmishController game, ShipLibrary lib, Camera cam)
        {
            this.game = game; this.lib = lib; this.cam = cam;
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var canvasGo = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler));
            var canvas = canvasGo.GetComponent<Canvas>();
            // Screen-space-camera so the HUD is also in camera captures (tests, recordings).
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = cam.nearClipPlane + 5f;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            var root = canvasGo.transform;

            status = MakeText(root, "status", 26, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(28, -22), new Vector2(900, 40));
            MakeText(root, "hint", 20, TextAnchor.UpperRight, new Vector2(1, 1), new Vector2(-28, -24), new Vector2(600, 40)).text =
                "<color=#8d98ab>H  controls     P  pause     + / -  speed</color>";

            banner = MakeText(root, "banner", 72, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.62f), Vector2.zero, new Vector2(1200, 120));
            banner.fontStyle = FontStyle.Bold;

            // Selected ships, bottom left.
            selectionPanel = MakeRect(root, "selection", new Vector2(0, 0), new Vector2(24, 24), new Vector2(760, 196));
            selectionPanel.gameObject.AddComponent<Image>().color = Panel;
            for (int i = 0; i < MaxCells; i++)
            {
                int col = i % 8, row = i / 8;
                var cell = MakeRect(selectionPanel, "cell", new Vector2(0, 1), new Vector2(12 + col * 92, -12 - row * 60), new Vector2(84, 54));
                cell.gameObject.AddComponent<Image>().color = new Color(1, 1, 1, 0.05f);
                var icon = MakeRect(cell, "icon", new Vector2(0.5f, 0.5f), new Vector2(0, 4), new Vector2(46, 46)).gameObject.AddComponent<Image>();
                icon.preserveAspect = true;
                var sh = Bar(cell, -18, new Color(0.45f, 0.75f, 1f));
                var hp = Bar(cell, -23, new Color(0.35f, 0.95f, 0.45f));
                cells.Add((cell.gameObject, icon, hp, sh));
            }
            detail = MakeText(selectionPanel, "detail", 20, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(110, -14), new Vector2(630, 170));

            // Controls card, shown at the start and on H.
            helpPanel = MakeRect(root, "help", new Vector2(1, 0), new Vector2(-24, 24), new Vector2(560, 330));
            helpPanel.gameObject.AddComponent<Image>().color = Panel;
            help = MakeText(helpPanel, "text", 20, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(20, -16), new Vector2(520, 300));
            help.text =
                "<b>You command Blue.</b>\n" +
                "<color=#a8d4ff>Select</color>   left-click, or drag a box. Shift adds.\n" +
                "            Double-click: all of that type on screen. Ctrl+A: all.\n" +
                "<color=#a8d4ff>Orders</color>   right-click space to move, an enemy to attack.\n" +
                "            Shift+right-click queues waypoints. X stops.\n" +
                "<color=#a8d4ff>Groups</color>   Ctrl+1..9 saves, 1..9 selects, press twice to jump.\n" +
                "<color=#a8d4ff>Camera</color>   WASD / arrows / screen edge, wheel zoom,\n" +
                "            Q/E or middle-drag to turn, F to focus.\n" +
                "<color=#8d98ab>H hides this card.</color>";
            helpUntil = 15f;
        }

        public void ToggleHelp() => helpUntil = helpPanel.gameObject.activeSelf ? 0f : float.MaxValue;

        public bool PointerOverPanel()
        {
            var m = (Vector2)Input.mousePosition;
            return (selectionPanel.gameObject.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(selectionPanel, m, cam))
                || (helpPanel.gameObject.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(helpPanel, m, cam));
        }

        public void Refresh()
        {
            var w = game.World;
            int t = (int)w.Time;
            var sb = new StringBuilder();
            sb.AppendFormat("<color=#a8d4ff>BLUE  {0}</color>      <color=#ffb0a8>RED  {1}</color>      {2:00}:{3:00}", w.Count(0), w.Count(1), t / 60, t % 60);
            if (game.Paused) sb.Append("      <color=#e7b04c>PAUSED</color>");
            else if (game.GameSpeed != 1f) sb.AppendFormat("      <color=#e7b04c>×{0:0.##}</color>", game.GameSpeed);
            status.text = sb.ToString();

            banner.text = w.Winner < 0 ? "" : w.Winner == SkirmishController.PlayerTeam
                ? "<color=#a8ffc0>VICTORY</color>" : "<color=#ffb0a8>DEFEAT</color>";

            helpPanel.gameObject.SetActive(Time.unscaledTime < helpUntil);
            RefreshSelection();
        }

        void RefreshSelection()
        {
            var sel = game.Selected;
            selectionPanel.gameObject.SetActive(sel.Count > 0);
            if (sel.Count == 0) return;
            bool single = sel.Count == 1;
            detail.gameObject.SetActive(single);
            int rows = single ? 1 : Mathf.Min(3, (sel.Count + 7) / 8);
            selectionPanel.sizeDelta = new Vector2(single ? 620 : 12 + Mathf.Min(8, sel.Count) * 92, single ? 112 : 12 + rows * 60);
            for (int i = 0; i < cells.Count; i++)
            {
                var (cell, icon, hp, sh) = cells[i];
                bool on = i < sel.Count && (!single || i == 0);
                cell.SetActive(on);
                if (!on) continue;
                var s = sel[i];
                icon.sprite = lib.Icon(s.Def.id);
                SetBar(hp, (float)s.Armor / s.StartArmor, true);
                SetBar(sh, (float)s.Shield / s.StartShield, false);
            }
            if (single)
            {
                var s = sel[0];
                var guns = s.Guns.GroupBy(g => g.Weapon.id).Select(g => (g.Count() > 1 ? g.Count() + " × " : "") + Pretty(g.Key));
                detail.text = string.Format(
                    "<b>{0}</b>   <color=#8d98ab>{1}</color>\n" +
                    "Armour  {2:N0} / {3:N0}     Shield  {4:N0} / {5:N0}\n" +
                    "Speed  {6:0}     <color=#8d98ab>{7}</color>\n" +
                    "{8}",
                    s.Def.name, Pretty(s.Def.hull), Mathf.Max(0, s.Armor), s.StartArmor, Mathf.Max(0, s.Shield), s.StartShield,
                    s.Def.speed, s.InShieldField ? "in shield field" : "",
                    string.Join(",  ", guns));
            }
        }

        static string Pretty(string id) => string.Join(" ", id.Split('_').Select(w => char.ToUpper(w[0]) + w.Substring(1)));

        static void SetBar(Image bar, float f, bool armour)
        {
            f = Mathf.Clamp01(f);
            var rt = bar.rectTransform;
            rt.sizeDelta = new Vector2(72f * f, rt.sizeDelta.y);
            if (armour)
                bar.color = f > 0.5f ? Color.Lerp(new Color(1f, 0.85f, 0.2f), new Color(0.35f, 0.95f, 0.45f), (f - 0.5f) * 2f)
                                     : Color.Lerp(new Color(1f, 0.25f, 0.2f), new Color(1f, 0.85f, 0.2f), f * 2f);
        }

        Image Bar(RectTransform parent, float y, Color c)
        {
            var r = MakeRect(parent, "bar", new Vector2(0.5f, 0.5f), new Vector2(0, y), new Vector2(72, 3));
            r.pivot = new Vector2(0, 0.5f);
            r.anchoredPosition = new Vector2(-36, y);
            var img = r.gameObject.AddComponent<Image>();
            img.color = c;
            return img;
        }

        static RectTransform MakeRect(Transform parent, string name, Vector2 anchor, Vector2 pos, Vector2 size)
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

        Text MakeText(Transform parent, string name, int size, TextAnchor align, Vector2 anchor, Vector2 pos, Vector2 box)
        {
            var r = MakeRect(parent, name, anchor, pos, box);
            var t = r.gameObject.AddComponent<Text>();
            t.font = font; t.fontSize = size; t.alignment = align; t.color = TextColour; t.supportRichText = true;
            t.horizontalOverflow = HorizontalWrapMode.Overflow; t.verticalOverflow = VerticalWrapMode.Overflow;
            return t;
        }
    }
}
