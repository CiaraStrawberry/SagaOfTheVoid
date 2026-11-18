using System;
using System.Collections.Generic;
using System.Linq;
using Saga.Sim;
using UnityEngine;
using UnityEngine.UI;

namespace Saga.Game
{
    // The guided practice battle (start menu: Tutorial). A coach panel at the top of the screen gives one step at
    // a time; each step lists what to do and ticks each thing off when the player has done it, then moves on.
    // Red is driven by this script, not the computer player: ships sit still until a step sends them in, and the
    // battle ends when the last step is done (World.ScriptedEnd), not when Red happens to run out of ships.
    public class Tutorial
    {
        public const string DoneKey = "saga.tutorial.done";
        const int Red = 1;

        class Check
        {
            public string Label;
            public Func<bool> Done;
            public bool Ticked;
        }

        class Step
        {
            public string Title, Body;
            public readonly List<Check> Checks = new List<Check>();
            public Action Enter;
            public bool Final;
            public Step Do(string label, Func<bool> done) { Checks.Add(new Check { Label = label, Done = done }); return this; }
        }

        readonly SkirmishController game;
        readonly List<Step> steps = new List<Step>();
        readonly List<Ship> attackers = new List<Ship>(); // Red ships this script sends at the player
        World world;
        int index = -1;
        float doneFor = -1f;      // seconds since every check of the step ticked (-1: not yet)
        float orderTimer;

        // what the player had when the step began, to measure from
        Vector3 camStart; float zoomStart, yawStart;
        int recallsStart;
        Vector3 beaconAt;
        Ship frigate, capital;
        static readonly HashSet<string> AntiShip = new HashSet<string> { "laser_frigate", "cannon_frigate" };
        readonly List<Ship> enemyFighters = new List<Ship>();
        double stepTime;

        // coach panel
        RectTransform panel;
        Text header, title, body, checks, footer;
        GameObject nextButton, skipLink;

        public Tutorial(SkirmishController game) { this.game = game; BuildSteps(); }

        public int StepIndex => index;
        public int StepCount => steps.Count;
        public Vector3 BeaconAt => beaconAt;
        public string StepTitle => index >= 0 && index < steps.Count ? steps[index].Title : "";

        // Blue's starting fleet on the left; Red starts empty. Nobody earns money unless a step hands it out.
        public void SetUpWorld(World w)
        {
            world = w;
            w.PlayerTeams.Add(SkirmishController.PlayerTeam);
            w.ScriptedTeams.Add(Red);
            w.ScriptedEnd = true;
            var me = w.TeamOf(SkirmishController.PlayerTeam);
            me.Money = 0; me.IncomePerSecond = 0;
            var them = w.TeamOf(Red);
            them.Money = 0; them.IncomePerSecond = 0; them.BuysShips = false;
            var fleet = new[] { "destroyer", "cannon_frigate", "missile_frigate", "fighter", "fighter" };
            var at = new[] { new Vec2(-4000, 0), new Vec2(-4500, -800), new Vec2(-4500, 800), new Vec2(-3300, -1300), new Vec2(-3300, 1300) };
            for (int i = 0; i < fleet.Length; i++) w.Spawn(fleet[i], SkirmishController.PlayerTeam, at[i], -Math.PI / 2);
        }

        // ---------- the steps ----------

        void BuildSteps()
        {
            steps.Add(new Step
            {
                Title = "Welcome, commander",
                Body = "This short practice battle teaches you the controls, one step at a time. Each step waits until you have done it.\n" +
                       "Your fleet is the <color=#a8d4ff>blue</color> ships. Nothing will attack you until you're ready.",
            });

            steps.Add(new Step
            {
                Title = "Look around",
                Body = "First, get the camera where you want it.",
                Enter = () => { camStart = game.CameraPivot; zoomStart = game.CameraDistance; yawStart = game.CameraYaw; },
            }
            .Do("Move the camera: <b>W A S D</b>, the arrow keys, or push the mouse to the edge of the screen",
                () => Flat(game.CameraPivot - camStart).magnitude > 1200f)
            .Do("Zoom: <b>mouse wheel</b>", () => Mathf.Abs(game.CameraDistance / zoomStart - 1f) > 0.2f)
            .Do("Turn: <b>Q</b> and <b>E</b>, or hold the middle mouse button and drag",
                () => Mathf.Abs(Mathf.DeltaAngle(game.CameraYaw, yawStart)) > 25f));

            steps.Add(new Step
            {
                Title = "Select ships",
                Body = "You give orders to the ships you have selected. Selected ships get a ring around them.\n" +
                       "<color=#8d98ab>Tip: press F to bring the camera to your selection.</color>",
            }
            .Do("<b>Left-click</b> one of your ships", () => game.Selected.Count == 1 && !game.Inspecting)
            .Do("<b>Drag a box</b> around your whole fleet (or press <b>Ctrl+A</b>)",
                () => !game.Inspecting && game.Selected.Count >= game.Own.Count() && game.Own.Count() > 1));

            steps.Add(new Step
            {
                Title = "Move",
                Body = "With ships selected, <b>right-click</b> empty space to send them there. They keep their formation.\n" +
                       "<color=#8d98ab>Tip: Shift + right-click adds waypoints, and X stops them.</color>",
                Enter = () =>
                {
                    beaconAt = BattleView.ToWorld(Centre(game.Own)) + new Vector3(3500f, 0, 1800f);
                    game.View.ShowBeacon(beaconAt);
                },
            }
            .Do("Move your fleet to the <color=#ffc860>amber ring</color>", () =>
            {
                var big = game.Own.Where(s => !Hulls.IsSmall(s.Hull)).ToList();
                var c = BattleView.ToWorld(Centre(big.Count > 0 ? big : game.Own.ToList()));
                return Flat(c - beaconAt).magnitude < 1300f;
            }));

            steps.Add(new Step
            {
                Title = "Groups",
                Body = "Save ships as a numbered group to pick them again in one key press.\n" +
                       "<color=#8d98ab>Tip: press the number twice quickly to jump the camera to the group.</color>",
                Enter = () => { game.View.ShowBeacon(null); recallsStart = game.GroupRecalls; },
            }
            .Do("Select some ships and press <b>Ctrl+1</b> to save them as group 1", () => Enumerable.Range(1, 9).Any(game.HasGroup))
            .Do("Press <b>Esc</b> to clear the selection, then <b>1</b> to get the group back", () => game.GroupRecalls > recallsStart));

            steps.Add(new Step
            {
                Title = "Attack",
                Body = "A <color=#ffb0a8>red</color> enemy frigate has turned up, marked with the amber ring. " +
                       "Select your ships and <b>right-click</b> it to attack.\n" +
                       "<color=#8d98ab>Bars over a ship: blue is its shield, green is its armour. At zero armour it's destroyed.</color>",
                Enter = () =>
                {
                    var c = Centre(game.Own);
                    frigate = WarpIn("cannon_frigate", new Vec2(c.X + 4800, c.Y + 500));
                    LookBetween(frigate);
                },
            }
            .Do("Order an attack on the frigate", () => frigate.Dead || game.Own.Any(s => s.OrderedTarget == frigate))
            .Do("Destroy it", () => frigate.Dead));

            steps.Add(new Step
            {
                Title = "Fighters",
                Body = "Enemy fighters are coming! Small craft fly in squadrons and swarm each other in a dogfight " +
                       "(the ring that appears around the fight).\n" +
                       "<b>Double-click</b> one of your fighters to select every fighter on screen, then <b>right-click</b> the enemy fighters.",
                Enter = () =>
                {
                    game.View.ShowBeacon(null);
                    var c = Centre(game.Own);
                    enemyFighters.Clear();
                    enemyFighters.Add(WarpIn("fighter", new Vec2(c.X + 3500, c.Y - 1800)));
                    enemyFighters.Add(WarpIn("scout", new Vec2(c.X + 3800, c.Y - 1300)));
                    attackers.AddRange(enemyFighters);
                },
            }
            .Do("Select only your fighters", () => !game.Inspecting && game.Selected.Count > 0 && game.Selected.All(s => Hulls.IsSmall(s.Hull)))
            .Do("Shoot down the enemy craft", () => enemyFighters.All(s => s.Dead)));

            steps.Add(new Step
            {
                Title = "Build a ship",
                Body = "Credits (top left) buy new ships from the build bar, bottom right. You've been given 1,200, and more keep coming in.\n" +
                       "<b>Left-click</b> a ship there to buy it. Right-click cancels. Hover over one to see what it is.",
                Enter = () =>
                {
                    var me = world.TeamOf(SkirmishController.PlayerTeam);
                    me.Money = Math.Max(me.Money, 1200);
                    // From here on credits keep coming in, as in a skirmish, so a lost ship or a wrong buy can't
                    // leave you stuck with nothing to build.
                    me.IncomePerSecond = World.Income;
                    stepTime = world.Time;
                },
            }
            .Do("Buy a ship (a <b>Fighter</b> costs 950)", () => Bought())
            .Do("Wait while it's built. <b>+</b> speeds the game up, <b>-</b> slows it down", () =>
            {
                var me = world.TeamOf(SkirmishController.PlayerTeam);
                return me.Reserve.Count > 0 || game.Own.Any(s => s.Arrived >= stepTime);
            }));

            steps.Add(new Step
            {
                Title = "Warp it in",
                Body = "New ships wait in your <b>Reserve</b>, just above the build bar, until you choose where they jump in.\n" +
                       "<color=#8d98ab>Tip: R warps in everything in the reserve at once.</color>",
            }
            .Do("<b>Click</b> the ship in the Reserve, then <b>click</b> near your fleet", () => game.Own.Any(s => s.Arrived >= 0)));

            steps.Add(new Step
            {
                Title = "Big guns for big ships",
                Body = "An enemy <color=#ffb0a8>capital ship</color> has jumped in: huge armour and shields. Two <b>Laser Frigates</b> have joined you to deal with it.\n" +
                       "Laser and Cannon Frigates are <b>anti-ship</b>: they hit big ships very hard but almost never hit fighters. " +
                       "Point them at the biggest target. <color=#8d98ab>(Missile Frigates go after other frigates; fighters and Flak Frigates deal with small craft.)</color>",
                Enter = () =>
                {
                    var c = Centre(game.Own);
                    capital = WarpIn("capital", new Vec2(c.X + 5500, c.Y));
                    foreach (var dy in new[] { -500.0, 500.0 })
                    {
                        var s = world.Spawn("laser_frigate", SkirmishController.PlayerTeam, new Vec2(c.X - 900, c.Y + dy), -Math.PI / 2);
                        s.Arrived = world.Time;
                    }
                    LookBetween(capital);
                },
            }
            .Do("Select your <b>Laser Frigates</b> (double-click one), adding any Cannon Frigates with Shift-click",
                () => !game.Inspecting && game.Selected.Count > 0 && game.Selected.All(s => AntiShip.Contains(s.Def.id))
                      && game.Selected.Any(s => s.Def.id == "laser_frigate"))
            .Do("<b>Right-click</b> the capital ship to attack it", () => capital.Dead || game.Own.Any(s => AntiShip.Contains(s.Def.id) && s.OrderedTarget == capital))
            .Do("Destroy the capital ship", () => capital.Dead));

            steps.Add(new Step
            {
                Title = "Your first battle",
                Body = "A <color=#ffb0a8>Red patrol</color> is heading for you. Destroy it!\n" +
                       "<color=#8d98ab>Tips: T then click is attack-move: they fight whatever they meet on the way. " +
                       "P pauses if you need to think. H shows every control.</color>",
                Final = true,
                Enter = () =>
                {
                    var c = Centre(game.Own);
                    foreach (var (id, dy) in new[] { ("cannon_frigate", -600.0), ("missile_frigate", 600.0), ("fighter", -1400.0), ("bomber", 1400.0) })
                        attackers.Add(WarpIn(id, new Vec2(c.X + 6500, c.Y + dy)));
                },
            }
            .Do("Destroy every Red ship", () => world.Count(Red) == 0));
        }

        // A Red ship jumping in (with the warp effect), facing Blue.
        Ship WarpIn(string id, Vec2 at)
        {
            var s = world.Spawn(id, Red, at, Math.PI / 2);
            s.Arrived = world.Time;
            return s;
        }

        // Swings the camera to halfway between the fleet and a new arrival, far enough out to see both.
        void LookBetween(Ship target)
        {
            var fleet = BattleView.ToWorld(Centre(game.Own));
            var there = BattleView.ToWorld(target.Pos);
            game.LookAt((fleet + there) * 0.5f, Vector3.Distance(fleet, there) * 1.3f);
        }

        bool Bought()
        {
            var me = world.TeamOf(SkirmishController.PlayerTeam);
            return me.Queue.Count > 0 || me.Reserve.Count > 0 || game.Own.Any(s => s.Arrived >= stepTime);
        }

        // ---------- running ----------

        public void Update(float dt)
        {
            if (world == null || panel == null) return;
            if (index < 0) Go(0);
            DriveRed(dt);
            // The amber ring: the move goal, then the ship a step wants attacked.
            var marked = StepTitle == "Attack" ? frigate : StepTitle == "Big guns for big ships" ? capital : null;
            if (StepTitle == "Move" && world.Winner < 0) game.View.ShowBeacon(beaconAt);
            else if (marked != null && !marked.Dead) game.View.ShowBeacon(BattleView.ToWorld(marked.Pos), marked == capital ? 1500f : 900f);
            else game.View.ShowBeacon(null);

            // Lost the whole fleet with nothing left to bring in: that's a defeat (Play again restarts).
            var me = world.TeamOf(SkirmishController.PlayerTeam);
            if (world.Winner < 0 && !game.Own.Any() && me.Reserve.Count == 0 && me.Queue.Count == 0 && index > 0)
                world.Winner = Red;
            if (world.Winner >= 0) { ShowEnd(); return; }

            var step = steps[index];
            foreach (var c in step.Checks) if (!c.Ticked && c.Done()) { c.Ticked = true; SoundBoard.Current?.Click(); }
            if (step.Checks.Count > 0 && step.Checks.All(c => c.Ticked))
            {
                if (doneFor < 0) doneFor = 0;
                doneFor += dt;
                if (doneFor > 1.4f)
                {
                    if (step.Final) Win();
                    else Go(index + 1);
                }
            }
            Draw();
        }

        // Moves to step i (also used by Skip and the tests).
        public void Go(int i)
        {
            index = Mathf.Clamp(i, 0, steps.Count - 1);
            doneFor = -1f;
            steps[index].Enter?.Invoke();
            Draw();
        }

        public void Skip()
        {
            if (index < steps.Count - 1) Go(index + 1);
            else Win();
        }

        // Done: the battle ends in Blue's favour and the menu ticks the tutorial off.
        void Win()
        {
            world.Winner = SkirmishController.PlayerTeam;
            PlayerPrefs.SetInt(DoneKey, 1);
            PlayerPrefs.Save();
        }

        // Red ships this script has sent in attack-move at the middle of the player's fleet, re-aimed every 2 s.
        void DriveRed(float dt)
        {
            attackers.RemoveAll(s => s.Dead);
            if (attackers.Count == 0 || game.Paused) return;
            orderTimer -= dt;
            if (orderTimer > 0) return;
            orderTimer = 2f;
            var own = game.Own.ToList();
            if (own.Count == 0) return;
            world.OrderMove(attackers, Centre(own), false, attack: true);
        }

        // ---------- the coach panel ----------

        public void Show(Camera cam)
        {
            var root = UiKit.Canvas("Tutorial", cam, 12).transform;
            panel = UiKit.Box(root, "coach", new Vector2(0.5f, 1), new Vector2(0, -100), new Vector2(860, 260), UiKit.Panel).rectTransform;
            var edge = UiKit.Box(panel, "edge", new Vector2(0, 1), Vector2.zero, new Vector2(5, 260), new Color(1f, 0.78f, 0.35f, 0.9f));
            edge.raycastTarget = false;
            header = UiKit.Label(panel, "header", 17, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(28, -16), new Vector2(600, 24));
            header.color = new Color(1f, 0.78f, 0.35f);
            title = UiKit.Label(panel, "title", 30, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(28, -40), new Vector2(800, 40));
            title.fontStyle = FontStyle.Bold;
            body = UiKit.Label(panel, "body", 21, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(28, -86), new Vector2(804, 100));
            checks = UiKit.Label(panel, "checks", 21, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(28, -150), new Vector2(804, 100));
            footer = UiKit.Label(panel, "footer", 20, TextAnchor.LowerLeft, new Vector2(0, 0), new Vector2(28, 16), new Vector2(500, 28));
            var next = UiKit.Button(panel, "Start", new Vector2(1, 0), new Vector2(-20, 14), new Vector2(170, 46), () => Go(index + 1), 22);
            nextButton = next.transform.parent.gameObject;
            var skip = UiKit.Box(panel, "skip", new Vector2(1, 1), new Vector2(-20, -14), new Vector2(150, 28), new Color(0, 0, 0, 0));
            var sb = skip.gameObject.AddComponent<Button>();
            sb.targetGraphic = skip;
            sb.onClick.AddListener(() => { SoundBoard.Current?.Click(); Skip(); });
            UiKit.Label(skip.transform, "label", 18, TextAnchor.MiddleRight, new Vector2(1, 0.5f), Vector2.zero, new Vector2(150, 28), "Skip step  ›").color = UiKit.Muted;
            skipLink = skip.gameObject;
        }

        void Draw()
        {
            if (panel == null || index < 0) return;
            var step = steps[index];
            header.text = string.Format("TUTORIAL   ·   STEP {0} OF {1}", index + 1, steps.Count);
            title.text = step.Title;
            body.text = step.Body;
            float y = 86 + body.preferredHeight + 14;
            checks.rectTransform.anchoredPosition = new Vector2(28, -y);
            checks.text = string.Join("\n", step.Checks.Select(c => c.Ticked
                ? "<color=#58c98f>✓</color>  <color=#8d98ab>" + c.Label + "</color>"
                : "<color=#ffc860>›</color>  " + c.Label));
            if (step.Checks.Count > 0) y += checks.preferredHeight + 10;
            bool info = step.Checks.Count == 0;
            nextButton.SetActive(info);
            skipLink.SetActive(!info);
            footer.text = doneFor >= 0 ? (step.Final ? "<color=#58c98f><b>Tutorial complete!</b></color>" : "<color=#58c98f><b>Nicely done.</b></color>") : "";
            float h = y + (info || doneFor >= 0 ? 70 : 26);
            panel.sizeDelta = new Vector2(860, h);
            panel.Find("edge").GetComponent<RectTransform>().sizeDelta = new Vector2(5, h);
        }

        // After the battle ends: a closing word (won) or nothing (the HUD's defeat banner says it).
        void ShowEnd()
        {
            if (world.Winner != SkirmishController.PlayerTeam) { panel.gameObject.SetActive(false); return; }
            header.text = "TUTORIAL COMPLETE";
            title.text = "You're ready";
            body.text = "That's everything you need to play. Try a <b>Skirmish</b> against the computer, or start the <b>Campaign</b> from the main menu.\n" +
                        "<color=#8d98ab>H shows the full list of controls in any battle.</color>";
            checks.text = ""; footer.text = "";
            nextButton.SetActive(false); skipLink.SetActive(false);
            float h = 86 + body.preferredHeight + 40;
            panel.sizeDelta = new Vector2(860, h);
            panel.Find("edge").GetComponent<RectTransform>().sizeDelta = new Vector2(5, h);
        }

        static Vec2 Centre(IEnumerable<Ship> ships)
        {
            var l = ships.ToList();
            return l.Count == 0 ? new Vec2(0, 0) : new Vec2(l.Average(s => s.Pos.X), l.Average(s => s.Pos.Y));
        }

        static Vector3 Flat(Vector3 v) { v.y = 0; return v; }
    }
}
