using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saga.Sim;
using UnityEngine;
using UnityEngine.EventSystems;
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
        readonly RectTransform selectionPanel, helpPanel, buildPanel;
        readonly Text tooltip, queueText, objective;
        GameObject endPanel;
        RectTransform reservePanel;
        Text reserveHint;
        readonly List<(string id, RectTransform slot, Text count, Image frame)> reserveSlots = new List<(string, RectTransform, Text, Image)>();
        float buildPanelTop;
        readonly Image queueFill;
        readonly List<(string id, Image icon, Text cost, Text count, Image shade)> buildButtons = new List<(string, Image, Text, Text, Image)>();
        string hovered;
        readonly List<(GameObject cell, Image icon, Image hp, Image sh)> cells = new List<(GameObject, Image, Image, Image)>();
        float helpUntil;

        public SkirmishHud(SkirmishController game, ShipLibrary lib, Camera cam)
        {
            this.game = game; this.lib = lib; this.cam = cam;
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            if (EventSystem.current == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            var canvasGo = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            // Screen-space-camera so the HUD is also in camera captures (tests, recordings).
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = cam.nearClipPlane + 5f;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<AspectFit>(); // narrower than 16:9: scale to fit the width
            var root = canvasGo.transform;

            status = MakeText(root, "status", 26, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(28, -22), new Vector2(900, 40));
            // Menu: pauses the battle and opens the pause menu (also Escape with nothing selected).
            menuLabel = UiKit.Button(root, "Menu", new Vector2(1, 1), new Vector2(-24, -16), new Vector2(150, 38), game.OpenMenu, 20);
            BuildPauseMenu(cam);
            MakeText(root, "hint", 20, TextAnchor.UpperRight, new Vector2(1, 1), new Vector2(-190, -24), new Vector2(600, 40)).text =
                "<color=#8d98ab>H  controls     P  pause     + / -  speed</color>";

            objective = MakeText(root, "objective", 22, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(28, -60), new Vector2(1100, 36));
            banner = MakeText(root, "banner", 72, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.62f), Vector2.zero, new Vector2(1200, 120));
            banner.fontStyle = FontStyle.Bold;

            // Selected ships, bottom left.
            selectionPanel = MakeRect(root, "selection", new Vector2(0, 0), new Vector2(24, 24), new Vector2(760, 196));
            selectionPanel.gameObject.AddComponent<Image>().color = Panel;
            for (int i = 0; i < MaxCells; i++)
            {
                int col = i % 8, row = i / 8;
                var cell = MakeRect(selectionPanel, "cell", new Vector2(0, 1), new Vector2(12 + col * 92, -12 - row * 60), new Vector2(84, 54));
                var cellImage = cell.gameObject.AddComponent<Image>();
                cellImage.color = new Color(1, 1, 1, 0.05f);
                // Click a ship in the group to select just that one; Shift-click drops it from the group.
                int index = i;
                var pick = cell.gameObject.AddComponent<Button>();
                pick.targetGraphic = cellImage;
                pick.onClick.AddListener(() => game.PickFromSelection(index, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)));
                var icon = MakeRect(cell, "icon", new Vector2(0.5f, 0.5f), new Vector2(0, 4), new Vector2(46, 46)).gameObject.AddComponent<Image>();
                icon.preserveAspect = true;
                var sh = Bar(cell, -18, new Color(0.45f, 0.75f, 1f));
                var hp = Bar(cell, -23, new Color(0.35f, 0.95f, 0.45f));
                cells.Add((cell.gameObject, icon, hp, sh));
            }
            detail = MakeText(selectionPanel, "detail", 20, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(110, -14), new Vector2(630, 170));

            // Build bar, bottom right: two rows of six ships. Left-click buys, right-click cancels one.
            var buyable = lib.ships.Where(e => game.World.Rules.Ships.TryGetValue(e.id, out var d) && d.buyable)
                                   .OrderBy(e => System.Array.IndexOf(Saga.Sim.World.BuyOrder, e.id)).ToList();
            buildPanel = MakeRect(root, "build", new Vector2(1, 0), new Vector2(-24, 24), new Vector2(12 + 6 * 92, 12 + 2 * 108 + 54));
            buildPanel.gameObject.AddComponent<Image>().color = Panel;
            queueText = MakeText(buildPanel, "queue", 18, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(14, -10), new Vector2(520, 24));
            var qbar = MakeRect(buildPanel, "queue bar", new Vector2(0, 1), new Vector2(14, -38), new Vector2(528, 5));
            qbar.gameObject.AddComponent<Image>().color = new Color(1, 1, 1, 0.08f);
            queueFill = MakeRect(qbar, "fill", new Vector2(0, 0.5f), Vector2.zero, new Vector2(0, 5)).gameObject.AddComponent<Image>();
            queueFill.rectTransform.pivot = new Vector2(0, 0.5f);
            queueFill.color = new Color(0.45f, 0.75f, 1f);
            for (int i = 0; i < buyable.Count; i++)
            {
                var id = buyable[i].id;
                int col = i % 6, row = i / 6;
                var b = MakeRect(buildPanel, id, new Vector2(0, 1), new Vector2(12 + col * 92, -54 - row * 108), new Vector2(84, 100));
                b.gameObject.AddComponent<Image>().color = new Color(1, 1, 1, 0.06f);
                var ic = MakeRect(b, "icon", new Vector2(0.5f, 1), new Vector2(0, -4), new Vector2(64, 64)).gameObject.AddComponent<Image>();
                ic.sprite = lib.Icon(id); ic.preserveAspect = true; ic.raycastTarget = false;
                var cost = MakeText(b, "cost", 17, TextAnchor.LowerCenter, new Vector2(0.5f, 0), new Vector2(0, 6), new Vector2(84, 24));
                cost.text = game.World.Rules.Ships[id].cost.ToString("N0");
                cost.raycastTarget = false;
                var count = MakeText(b, "count", 18, TextAnchor.UpperRight, new Vector2(1, 1), new Vector2(-6, -4), new Vector2(40, 24));
                count.color = new Color(0.65f, 0.85f, 1f); count.raycastTarget = false;
                var shade = MakeRect(b, "shade", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(84, 100)).gameObject.AddComponent<Image>();
                shade.color = new Color(0.02f, 0.03f, 0.05f, 0.6f); shade.raycastTarget = false;
                var hooks = b.gameObject.AddComponent<UiHooks>();
                hooks.OnLeft = () => game.Buy(id);
                hooks.OnRight = () => game.CancelBuy(id);
                hooks.OnEnter = () => hovered = id;
                hooks.OnExit = () => { if (hovered == id) hovered = null; };
                buildButtons.Add((id, ic, cost, count, shade));
            }
            buildPanelTop = 24 + 12 + 2 * 108 + 54;
            tooltip = MakeText(root, "tooltip", 18, TextAnchor.LowerRight, new Vector2(1, 0), new Vector2(-28, buildPanelTop + 10), new Vector2(560, 60));

            // Reserve, above the build bar: built ships waiting to warp in. Click one, then click the battlefield.
            reservePanel = MakeRect(root, "reserve", new Vector2(1, 0), new Vector2(-24, buildPanelTop + 8), new Vector2(12 + 6 * 92, 128));
            reservePanel.gameObject.AddComponent<Image>().color = Panel;
            MakeText(reservePanel, "title", 18, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(14, -8), new Vector2(300, 24)).text =
                "<b>Reserve</b>  <color=#8d98ab>click a ship, then where it warps in</color>";
            var all = UiKit.Button(reservePanel, "Deploy all  (R)", new Vector2(1, 1), new Vector2(-10, -6), new Vector2(170, 30), () => game.BeginDeploy("*"), 17);
            all.transform.parent.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
            foreach (var e in buyable)
            {
                var id = e.id;
                var slot = MakeRect(reservePanel, "reserve " + id, new Vector2(0, 1), new Vector2(12, -40), new Vector2(84, 80));
                var frame = slot.gameObject.AddComponent<Image>();
                frame.color = new Color(1, 1, 1, 0.06f);
                var ic = MakeRect(slot, "icon", new Vector2(0.5f, 0.5f), new Vector2(0, 2), new Vector2(66, 66)).gameObject.AddComponent<Image>();
                ic.sprite = lib.Icon(id); ic.preserveAspect = true; ic.raycastTarget = false;
                var count = MakeText(slot, "count", 20, TextAnchor.UpperRight, new Vector2(1, 1), new Vector2(-6, -4), new Vector2(50, 26));
                count.color = new Color(0.65f, 0.85f, 1f);
                var hooks = slot.gameObject.AddComponent<UiHooks>();
                hooks.OnLeft = () => game.BeginDeploy(id);
                hooks.OnEnter = () => hovered = id;
                hooks.OnExit = () => { if (hovered == id) hovered = null; };
                reserveSlots.Add((id, slot, count, frame));
            }
            reserveHint = MakeText(root, "deploy hint", 24, TextAnchor.MiddleCenter, new Vector2(0.5f, 0), new Vector2(0, 150), new Vector2(1100, 40));

            // Controls card, shown at the start and on H.
            helpPanel = MakeRect(root, "help", new Vector2(1, 1), new Vector2(-24, -64), new Vector2(560, 380));
            helpPanel.gameObject.AddComponent<Image>().color = Panel;
            help = MakeText(helpPanel, "text", 20, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(20, -16), new Vector2(520, 330));
            help.text =
                "<b>You command Blue.</b>\n" +
                "<color=#a8d4ff>Select</color>   left-click, or drag a box. Shift adds.\n" +
                "            Double-click: all of that type on screen. Ctrl+A: all.\n" +
                "<color=#a8d4ff>Orders</color>   right-click space to move, an enemy to attack.\n" +
                "            Shift+right-click queues waypoints. X stops.\n" +
                "            T then click (or Ctrl+right-click): attack-move.\n" +
                "<color=#a8d4ff>Groups</color>   Ctrl+1..9 saves, 1..9 selects, press twice to jump.\n" +
                "<color=#a8d4ff>Camera</color>   WASD / arrows / screen edge, wheel zoom,\n" +
                "            Q/E or middle-drag to turn, F to focus.\n" +
                "<color=#a8d4ff>Game</color>       P pause, + / - speed, M mute, F5 restart, F10 quit.\n" +
                "<color=#8d98ab>H hides this card.</color>";
            helpUntil = 15f;
        }

        Text menuLabel;
        GameObject pauseMenu;

        // The pause menu: the battle stops beneath a dimmed screen until Resume (or Escape).
        void BuildPauseMenu(Camera cam)
        {
            var c = UiKit.Canvas("Pause menu", cam, 25);
            pauseMenu = c.gameObject;
            var r = c.transform;
            var dim = UiKit.Box(r, "dim", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(4000, 3000), new Color(0f, 0.01f, 0.03f, 0.6f));
            dim.raycastTarget = true;
            var panel = UiKit.Box(r, "panel", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(440, 520), UiKit.Panel).transform;
            var title = UiKit.Label(panel, "title", 44, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0, -28), new Vector2(400, 60), "Paused");
            title.fontStyle = FontStyle.Bold;
            float y = -110;
            void Add(string label, System.Action a) { UiKit.Button(panel, label, new Vector2(0.5f, 1f), new Vector2(0, y), new Vector2(360, 60), a, 24); y -= 74; }
            Add("Resume", game.CloseMenu);
            Add("Restart battle", () => SkirmishController.Begin(game.Config));
            Add("Controls", () => { game.CloseMenu(); helpUntil = Time.unscaledTime + 30f; });
            Add("Main menu", SkirmishController.BackToMenu);
            if (Application.platform != RuntimePlatform.WebGLPlayer) Add("Quit game", Application.Quit);
            var hintText = UiKit.Label(panel, "hint", 18, TextAnchor.LowerCenter, new Vector2(0.5f, 0f), new Vector2(0, 16), new Vector2(400, 26), "Escape to resume");
            hintText.color = UiKit.Muted;
            pauseMenu.SetActive(false);
        }

        public void ShowPauseMenu(bool show) => pauseMenu.SetActive(show);

        public void ToggleHelp() => helpUntil = helpPanel.gameObject.activeSelf ? 0f : float.MaxValue;

        public bool PointerOverPanel()
        {
            var m = (Vector2)Input.mousePosition;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return true;
            return (selectionPanel.gameObject.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(selectionPanel, m, cam))
                || (helpPanel.gameObject.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(helpPanel, m, cam))
                || RectTransformUtility.RectangleContainsScreenPoint(buildPanel, m, cam);
        }

        public void Refresh()
        {
            var w = game.World;
            int t = (int)w.Time;
            var sb = new StringBuilder();
            var me = w.TeamOf(SkirmishController.PlayerTeam);
            sb.AppendFormat("<color=#e7d38c>CREDITS  {0:N0}</color>  <color=#8d98ab>+{1:0}/s</color>      ", System.Math.Floor(me.Money), me.Money < Saga.Sim.World.MoneyCap ? me.IncomePerSecond : 0);
            if (me.Reserve.Count > 0) sb.AppendFormat("<color=#a8ffc0>RESERVE  {0}</color>      ", me.Reserve.Count);
            sb.AppendFormat("<color=#a8d4ff>BLUE  {0}</color><color=#8d98ab>/{4}</color>      <color=#ffb0a8>RED  {1}</color>      {2:00}:{3:00}", w.Count(0), w.Count(1), t / 60, t % 60, w.ShipCap - 1);
            if (game.Paused) sb.Append("      <color=#e7b04c>PAUSED</color>");
            else if (game.GameSpeed != 1f) sb.AppendFormat("      <color=#e7b04c>×{0:0.##}</color>", game.GameSpeed);
            status.text = sb.ToString();

            var mission = game.MissionDef;
            if (mission != null && w.Mission != null)
            {
                double left = System.Math.Max(0, w.Mission.TimeLimit - w.Time);
                string clock = string.Format("{0}:{1:00}", (int)left / 60, (int)left % 60);
                string goal = w.Mission.Objective == Saga.Sim.Objective.Survive ? "Survive  " + clock
                    : w.Mission.Objective == Saga.Sim.Objective.KillTarget ? "Destroy the enemy flagship"
                        + (w.Mission.Target != null && !w.Mission.Target.Dead ? string.Format("  <color=#8d98ab>({0:N0} armour left)</color>", w.Mission.Target.Armor) : "")
                    : string.Format("Destroy every enemy ship  <color=#8d98ab>{0} left · {1}</color>", w.Count(w.Mission.EnemyTeam), clock);
                objective.text = string.Format("<color=#e7b04c>MISSION {0}</color>  {1}   ·   {2}", mission.index + 1, mission.name, goal);
            }
            else if (w.KeyShips.TryGetValue(SkirmishController.PlayerTeam, out var mine) && w.KeyShips.TryGetValue(1, out var theirs))
            {
                string what = mine.Def.id == "home_base" ? "base" : "flagship";
                objective.text = string.Format("<color=#e7b04c>{0}</color>   Protect your {1}  <color=#8d98ab>({2:N0} armour)</color>   ·   Destroy theirs  <color=#8d98ab>({3:N0} armour)</color>",
                    what == "base" ? "BASE DEFENCE" : "CAPITAL SHIP", what, System.Math.Max(0, mine.Armor), System.Math.Max(0, theirs.Armor));
            }
            else objective.text = "";

            if (w.Winner < 0 || game.StoryBusy) banner.text = ""; // a won mission's aftermath plays first
            else
            {
                bool won = w.Winner == SkirmishController.PlayerTeam;
                string why = w.Mission?.Outcome != null && w.Mission.Outcome != "victory" ? "\n<size=30>" + w.Mission.Outcome + "</size>" : "";
                banner.text = won ? "<color=#a8ffc0>VICTORY</color>" : "<color=#ffb0a8>DEFEAT</color>" + why;
                if (endPanel == null) ShowEndButtons(won);
            }

            // The controls card waits until a transmission is over (it would sit behind the comms panel).
            if (game.StoryBusy && Time.unscaledTime < helpUntil) helpUntil = Time.unscaledTime + 12f;
            helpPanel.gameObject.SetActive(Time.unscaledTime < helpUntil && !game.StoryBusy && !game.MenuOpen);
            RefreshSelection();
            RefreshBuild();
            RefreshReserve();
        }

        void RefreshSelection()
        {
            var sel = game.Selected;
            selectionPanel.gameObject.SetActive(sel.Count > 0);
            if (sel.Count == 0) return;
            bool single = sel.Count == 1;
            detail.gameObject.SetActive(single);
            int rows = single ? 1 : Mathf.Min(3, (sel.Count + 7) / 8);
            selectionPanel.sizeDelta = new Vector2(single ? 620 : 12 + Mathf.Min(8, sel.Count) * 92, single ? 140 : 12 + rows * 60);
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
                    "{8}\n" +
                    "<color=#e7d38c>Damage dealt  {9:N0}     Kills  {10}</color>{11}",
                    s.Def.name, Pretty(s.Def.hull), Mathf.Max(0, s.Armor), s.StartArmor, Mathf.Max(0, s.Shield), s.StartShield,
                    s.Def.speed, s.ShieldBeamed ? "shield beam on it" : "",
                    string.Join(",  ", guns), s.DamageDealt, s.Kills,
                    s.ShieldsRestored > 0 ? string.Format("     <color=#a8ffc0>Shields restored  {0:N0}</color>", s.ShieldsRestored) : "");
            }
        }

        void RefreshBuild()
        {
            var w = game.World;
            var me = w.TeamOf(SkirmishController.PlayerTeam);
            foreach (var (id, icon, cost, count, shade) in buildButtons)
            {
                int queued = me.Queue.Where(o => o.ShipId == id).Sum(o => o.Count);
                count.text = queued > 0 ? "×" + queued : "";
                bool can = w.CannotBuy(SkirmishController.PlayerTeam, id) == null;
                shade.enabled = !can;
                cost.color = can ? TextColour : new Color(0.9f, 0.5f, 0.45f);
            }
            var b = me.Building;
            if (b != null)
            {
                double total = w.BuildTime(b.ShipId);
                queueText.text = string.Format("Building  <b>{0}</b>{1}   <color=#8d98ab>{2:0}s</color>{3}",
                    w.Rules.Ships[b.ShipId].name, b.Count > 1 ? " ×" + b.Count : "", total - b.Progress,
                    me.Queue.Count > 1 ? "   <color=#8d98ab>then " + string.Join(", ", me.Queue.Skip(1).Select(o => w.Rules.Ships[o.ShipId].name + (o.Count > 1 ? " ×" + o.Count : ""))) + "</color>" : "");
                queueFill.rectTransform.sizeDelta = new Vector2(528f * (float)(b.Progress / total), 5);
            }
            else
            {
                queueText.text = "<color=#8d98ab>Build: left-click buys, right-click cancels one</color>";
                queueFill.rectTransform.sizeDelta = new Vector2(0, 5);
            }
            if (hovered != null)
            {
                var d = w.Rules.Ships[hovered];
                var why = w.CannotBuy(SkirmishController.PlayerTeam, hovered);
                tooltip.text = string.Format("<b>{0}</b>   {1:N0} credits · {2:0} s to build\n<color=#8d98ab>{3} · armour {4:N0} · {5}</color>{6}",
                    d.name, d.cost, d.build_time, Pretty(d.hull), d.armor,
                    string.Join(", ", d.turrets.GroupBy(x => x.weapon).Select(g => (g.Count() > 1 ? g.Count() + " × " : "") + Pretty(g.Key))),
                    why != null ? "\n<color=#ef8a80>" + why + "</color>" : "");
            }
            else tooltip.text = "";
        }

        void RefreshReserve()
        {
            var me = game.World.TeamOf(SkirmishController.PlayerTeam);
            bool any = me.Reserve.Count > 0;
            reservePanel.gameObject.SetActive(any);
            int x = 0;
            foreach (var (id, slot, count, frame) in reserveSlots)
            {
                int n = me.InReserve(id);
                slot.gameObject.SetActive(n > 0);
                if (n == 0) continue;
                slot.anchoredPosition = new Vector2(12 + x * 92, -40);
                count.text = "×" + n;
                frame.color = game.DeployType == id ? new Color(0.45f, 1f, 0.6f, 0.35f) : new Color(1, 1, 1, 0.06f);
                x++;
            }
            var tp = tooltip.rectTransform.anchoredPosition;
            tooltip.rectTransform.anchoredPosition = new Vector2(tp.x, buildPanelTop + 10 + (any ? 140 : 0));
            if (game.Deploying)
            {
                string what = game.DeployType == "*" ? "the whole reserve (" + me.Reserve.Count + ")" : game.World.Rules.Ships[game.DeployType].name;
                reserveHint.text = "<color=#a8ffc0>Click to warp in " + what + "</color>   <color=#8d98ab>Shift keeps placing · right-click or Esc cancels</color>";
            }
            else if (game.AttackMoveArmed)
                reserveHint.text = "<color=#ffb0a0>Attack-move: click a spot</color>   <color=#8d98ab>they fight whatever they meet on the way · right-click or Esc cancels</color>";
            else reserveHint.text = "";
        }

        void ShowEndButtons(bool won)
        {
            var root = banner.canvas.transform;
            endPanel = UiKit.Rect(root, "end", new Vector2(0.5f, 0.45f), Vector2.zero, new Vector2(700, 80)).gameObject;
            var m = game.MissionDef;
            var cfg = game.Config;
            System.Action first; string firstLabel;
            if (m != null && won && m.index + 1 < 8) { firstLabel = "Next mission"; first = () => SkirmishController.Begin(new BattleConfig { Mission = m.index + 1 }); }
            else if (m != null && !won) { firstLabel = "Retry mission"; first = () => SkirmishController.Begin(cfg); }
            else { firstLabel = "Play again"; first = () => SkirmishController.Begin(cfg); }
            UiKit.Button(endPanel.transform, firstLabel, new Vector2(0, 0.5f), new Vector2(0, 0), new Vector2(330, 72), first, 28);
            UiKit.Button(endPanel.transform, "Menu", new Vector2(1, 0.5f), new Vector2(0, 0), new Vector2(330, 72), SkirmishController.BackToMenu, 28);
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
            t.raycastTarget = false; // text never blocks clicks on the battlefield
            return t;
        }
    }
}
