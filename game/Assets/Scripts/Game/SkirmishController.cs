using System.Collections.Generic;
using System.Linq;
using Saga.Sim;
using UnityEngine;

namespace Saga.Game
{
    // A playable skirmish: you command Blue (team 0) against the computer's Red.
    // Sits on the scene's camera; builds everything else at start.
    //
    // Camera   WASD / arrows / screen edge to pan, wheel to zoom, Q/E or middle-drag to turn, F to focus selection
    // Select   left-click, drag a box, Shift adds, double-click selects that type on screen, Esc clears
    // Groups   Ctrl+1..9 saves the selection, 1..9 recalls it (twice quickly to jump there)
    // Orders   right-click space to move (keeps formation), right-click an enemy to attack,
    //          Shift+right-click to queue waypoints, X to stop, Ctrl+A selects everything
    // Game     P pause, + / - speed, H help
    [RequireComponent(typeof(Camera))]
    public class SkirmishController : MonoBehaviour
    {
        public const int PlayerTeam = 0;
        const double Step = 1.0 / World.UpdateRate;

        [Tooltip("Use the small 8-ship fleets.")]
        public bool quickFleets;
        public int seed = 2017;
        [Tooltip("asteroid_field, space_station, ship_graveyard, or empty for open space")]
        public string mapId = "asteroid_field";
        [Tooltip("Skybox1, skybox2 ... skybox5")]
        public string skyId = "skybox5";

        public World World { get; private set; }
        public BattleView View { get; private set; }
        public readonly List<Ship> Selected = new List<Ship>();
        public float GameSpeed = 1f;
        public bool Paused;
        // Pause menu (Menu button, or Escape with nothing selected): the battle stops until it closes.
        public bool MenuOpen { get; private set; }
        bool pausedBeforeMenu;
        public void OpenMenu()
        {
            if (MenuOpen || World == null) return;
            MenuOpen = true; pausedBeforeMenu = Paused; Paused = true;
            hud.ShowPauseMenu(true);
        }
        public void CloseMenu()
        {
            if (!MenuOpen) return;
            MenuOpen = false; Paused = pausedBeforeMenu;
            hud.ShowPauseMenu(false);
        }

        Camera cam;
        SkirmishHud hud;
        double accumulator;
        readonly List<ShotEvent> shots = new List<ShotEvent>();
        readonly List<ImpactEvent> impacts = new List<ImpactEvent>();
        readonly List<Ship> deaths = new List<Ship>();
        readonly Dictionary<int, List<Ship>> groups = new Dictionary<int, List<Ship>>();
        int lastGroupKey = -1; float lastGroupTime;

        // camera rig: a point on the play plane, a turn, and a distance
        Vector3 pivot; float yaw, distance = 7000f;
        const float MinDist = 1200f, MaxDist = 24000f;

        // mouse selection
        Vector2 dragStart; bool dragging; bool leftDown;
        float lastClickTime; Ship lastClicked;

        [Tooltip("Show the start menu first. Tests turn this off to go straight into a battle.")]
        public bool showMenu = true;

        // The battle to start after the scene reloads (set by the menu and the end-of-battle buttons).
        public static BattleConfig Pending;
        public BattleConfig Config { get; private set; }
        public MissionDef MissionDef { get; private set; }
        StartMenu menu;
        bool resultSaved;
        // The guided practice battle, when Config.Tutorial.
        public Tutorial Tutorial { get; private set; }

        // Read-outs for the tutorial (and tests): where the camera looks, how far out, which way it faces.
        public Vector3 CameraPivot => pivot;
        public float CameraDistance => distance;
        public float CameraYaw => yaw;
        public bool HasGroup(int n) => groups.TryGetValue(n, out var g) && g.Count > 0;
        public int GroupRecalls { get; private set; }

        // Campaign story (CommsPanel): beats play as their triggers fire; the battle waits while one is open.
        StoryDef storyDef;
        StoryDirector story;
        CommsPanel comms;
        bool victoryStoryQueued;
        public const string EndingKey = "saga.campaign.ending";
        // True while a transmission is on screen or a won mission's aftermath is still to come.
        public CommsPanel Comms => comms;
        // Opens one of this mission's beats right away (tests and screenshots).
        public void PlayStoryBeat(string id) { var b = story?.Mission.beats.Find(x => x.id == id); if (b != null) comms.Play(b); }
        public bool StoryBusy => comms != null && (comms.Open || (World.Winner == PlayerTeam && !victoryStoryQueued));

        public static void Begin(BattleConfig config)
        {
            Pending = config;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        public static void BackToMenu()
        {
            Pending = null;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        void Start()
        {
            cam = GetComponent<Camera>();
            cam.fieldOfView = 35f;
            cam.nearClipPlane = 30f;
            cam.farClipPlane = 60000f;
            var lib = ShipLibrary.Load();
            var campaign = JsonUtility.FromJson<CampaignDef>(lib.campaignJson.text);

            var config = Pending ?? (showMenu ? null : new BattleConfig { Quick = quickFleets, Map = mapId, Sky = skyId });
            Pending = null;
            if (config == null)
            {
                // Menu over a quiet backdrop: the station map, slowly turning.
                BattleView.BuildStage(lib, cam, "space_station", "Skybox1");
                pivot = new Vector3(0, 0, 2000); distance = 13000f;
                PlaceCamera();
                new SoundBoard(lib, gameObject, cam).PlayMusic(lib.musicMenu);
                menu = new StartMenu(this, lib, cam, campaign);
                return;
            }
            Config = config;

            World = new World(lib.Rules(), seed);
            string map = config.Map, sky = config.Sky;
            if (config.Tutorial)
            {
                map = null; sky = "skybox5"; // open space: nothing in the way of the camera while learning it
                Tutorial = new Tutorial(this);
                Tutorial.SetUpWorld(World);
            }
            else if (config.Mission >= 0)
            {
                MissionDef = campaign.missions[config.Mission];
                World.SetUpMission(MissionDef, PlayerTeam, 1);
                map = MissionDef.map; sky = MissionDef.sky;
            }
            else
            {
                World.PlayerTeams.Add(PlayerTeam);
                Fleets.Spawn(World, Fleets.FleetFor(config), Fleets.GapFor(config));
                World.TeamOf(1).Difficulty = config.Difficulty;
                Fleets.SpawnMode(World, config.Mode, Fleets.GapFor(config));
            }

            BattleView.BuildStage(lib, cam, string.IsNullOrEmpty(map) ? null : map, sky);
            View = new BattleView(World, lib, cam);
            var board = new SoundBoard(lib, gameObject, cam);
            if (lib.battlePlaylist.Count > 0) board.PlayPlaylist(lib.battlePlaylist); else board.PlayMusic(lib.musicBattle);
            hud = new SkirmishHud(this, lib, cam);
            if (MissionDef != null && lib.narrativeJson != null)
            {
                storyDef = JsonUtility.FromJson<StoryDef>(lib.narrativeJson.text);
                var sm = storyDef.Mission(MissionDef.index);
                if (sm != null)
                {
                    story = new StoryDirector(sm, World, PlayerTeam, 1);
                    comms = new CommsPanel(lib, cam, storyDef, MissionDef.index);
                }
            }

            var mine = World.Alive.Where(s => s.Team == PlayerTeam).ToList();
            pivot = Centre(mine) + new Vector3(2000f, 0, 0);
            if (Tutorial != null) { pivot = Centre(mine) + new Vector3(1200f, 0, 0); distance = 6000f; hud.HideHelp(); Tutorial.Show(cam); }
            PlaceCamera();
        }

        void Update()
        {
            float dt = Time.unscaledDeltaTime;
            if (World == null)
            {
                // Menu backdrop: turn slowly.
                yaw += 3f * dt;
                PlaceCamera();
                if (Input.GetKeyDown(KeyCode.F10)) Application.Quit();
                return;
            }
            if (World.Winner >= 0 && !resultSaved)
            {
                resultSaved = true;
                if (MissionDef != null && World.Winner == PlayerTeam && MissionDef.index > StartMenu.Completed)
                {
                    PlayerPrefs.SetInt(StartMenu.ProgressKey, MissionDef.index);
                    PlayerPrefs.Save();
                }
            }
            SoundBoard.Current?.DuckMusic(comms != null && comms.Open);
            if (comms != null && UpdateStory(dt)) return;
            if (MenuOpen)
            {
                if (Input.GetKeyDown(KeyCode.Escape)) CloseMenu();
                PlaceCamera();
                View.Sync(shots, impacts, deaths, 0f, World.Time);
                hud.Refresh();
                return;
            }
            HandleKeys();
            HandleCamera(dt);
            HandleMouse();

            if (!Paused && World.Winner < 0)
            {
                accumulator += dt * GameSpeed;
                int guard = 0;
                while (accumulator >= Step && guard++ < 240)
                {
                    accumulator -= Step;
                    World.ClearEvents();
                    World.Step(Step);
                    shots.AddRange(World.Shots); impacts.AddRange(World.Impacts); deaths.AddRange(World.Deaths);
                }
                World.ClearEvents();
            }
            Selected.RemoveAll(s => s.Dead);
            foreach (var g in groups.Values) g.RemoveAll(s => s.Dead);

            PlaceCamera();
            View.Sync(shots, impacts, deaths, Paused ? 0f : dt * GameSpeed, World.Time);
            shots.Clear(); impacts.Clear(); deaths.Clear();
            hud.Refresh();
            Tutorial?.Update(dt);
        }

        // Starts any story beat that is due; while a transmission is open the battle stays frozen beneath it and
        // this returns true (no other input). On victory the aftermath plays before the end banner; after the
        // last mission comes the ending choice.
        bool UpdateStory(float dt)
        {
            if (!comms.Open)
            {
                if (World.Winner == PlayerTeam && !victoryStoryQueued)
                {
                    victoryStoryQueued = true;
                    foreach (var b in story.AtVictory()) comms.Play(b);
                    if (MissionDef.index == storyDef.missions.Count - 1 && storyDef.endings.Count > 0)
                        comms.Choose(storyDef.endings, e =>
                        {
                            PlayerPrefs.SetString(EndingKey, e.id);
                            PlayerPrefs.Save();
                            comms.Card(e.title, e.text);
                        });
                }
                else if (World.Winner < 0)
                {
                    var due = story.Due();
                    if (due != null) comms.Play(due, due.trigger == "mission_start" ? story.Mission : null);
                }
            }
            if (!comms.Open) return false;
            comms.Update(dt);
            PlaceCamera();
            View.Sync(shots, impacts, deaths, 0f, World.Time);
            hud.Refresh();
            return true;
        }

        // ---------- commands (public so tests and later UI can drive them) ----------

        public IEnumerable<Ship> Own => World.Alive.Where(s => s.Team == PlayerTeam);

        // inspect: select enemy ships to look at them (the panel shows their stats; no orders can be given).
        public void Select(IEnumerable<Ship> ships, bool add = false, bool inspect = false)
        {
            if (add && Inspecting != inspect) add = false; // never mix own and enemy ships
            if (!add) { foreach (var s in Selected) View.SetSelected(s, false); Selected.Clear(); }
            int before = Selected.Count;
            foreach (var s in ships)
                if (!s.Dead && (inspect ? s.Team != PlayerTeam : s.Team == PlayerTeam) && !Selected.Contains(s)) { Selected.Add(s); View.SetSelected(s, true); }
            if (Selected.Count > 0 && (Selected.Count != before || !add))
            {
                SoundBoard.Current?.Select();
                if (!inspect) Reply("select");
            }
        }

        // The selection is enemy ships being looked at, not commanded.
        public bool Inspecting => Selected.Count > 0 && Selected[0].Team != PlayerTeam;

        // Selection panel: click one ship of the group to select only it (shift: drop it from the group).
        public void PickFromSelection(int index, bool remove)
        {
            if (index < 0 || index >= Selected.Count) return;
            var s = Selected[index];
            if (remove) { Selected.Remove(s); View.SetSelected(s, false); }
            else Select(new[] { s }, false, s.Team != PlayerTeam);
        }

        public void SelectAllOwn() => Select(Own);

        public void CommandMove(Vector3 worldPoint, bool queue, bool attack = false)
        {
            if (Inspecting) return;
            if (Selected.Count == 0) return;
            World.OrderMove(Selected, BattleView.ToSim(worldPoint), queue, attack);
            Reply(attack ? "attack" : "move");
            View.Ping(worldPoint, attack);
        }

        // Attack-move: T, then left-click a spot (or Ctrl + right-click it).
        public bool AttackMoveArmed;

        public void CommandAttack(Ship target)
        {
            if (Inspecting) return;
            if (Selected.Count == 0 || target == null || target.Team == PlayerTeam) return;
            World.OrderAttack(Selected, target);
            Reply("attack");
            var v = View.ViewOf(target);
            if (v != null) View.Ping(v.Go.transform.position, true, v.Length * 1.3f);
        }

        // A spoken reply from the crew of the biggest selected ship (SoundBoard.Reply).
        void Reply(string kind)
        {
            if (Selected.Count == 0 || Inspecting) return;
            SoundBoard.Current?.Reply(Selected.Max(s => s.Hull), kind);
        }

        public void CommandStop() { if (!Inspecting) World.OrderStop(Selected); }

        // Into a dogfight area: fighters join the fight, other ships attack the enemies in it.
        public void CommandFight(Furball f)
        {
            if (Inspecting) return;
            if (Selected.Count == 0) return;
            World.OrderIntoFight(Selected, f, PlayerTeam);
            Reply("attack");
            View.Ping(BattleView.ToWorld(f.Centre), true, (float)f.Radius * 2f);
        }

        (Ship big, List<Ship> attackers)? StrikeUnderMouse()
        {
            var g = GroundUnderMouse();
            return g.HasValue ? View.StrikeZoneAt(g.Value) : null;
        }

        Furball FightUnderMouse()
        {
            var g = GroundUnderMouse();
            return g.HasValue ? World.FurballAt(BattleView.ToSim(g.Value)) : null;
        }

        // ---------- reserve: built ships wait until you choose where they warp in ----------

        public string DeployType { get; private set; }   // ship being placed, or "*" for the whole reserve
        public bool Deploying => DeployType != null;

        public void BeginDeploy(string shipId)
        {
            var me = World.TeamOf(PlayerTeam);
            if (shipId == "*" ? me.Reserve.Count > 0 : me.InReserve(shipId) > 0) DeployType = shipId;
        }

        public void CancelDeploy() { DeployType = null; View?.ShowDeployMarker(null, false, 0); }

        // Warps the chosen reserve ships in at a point on the play plane. Returns what arrived.
        public List<Ship> DeployAt(Vector3 worldPoint, string shipId)
        {
            var me = World.TeamOf(PlayerTeam);
            var ids = shipId == "*" ? me.Reserve.ToList() : new List<string> { shipId };
            var made = World.Deploy(PlayerTeam, ids, BattleView.ToSim(worldPoint));
            if (made.Count > 0) Select(made);
            return made;
        }

        void HandleDeploy(Vector2 mouse, bool shift)
        {
            var me = World.TeamOf(PlayerTeam);
            if (DeployType != "*" && me.InReserve(DeployType) == 0 || DeployType == "*" && me.Reserve.Count == 0) { CancelDeploy(); return; }
            var g = GroundUnderMouse();
            bool ok = g.HasValue && World.CannotDeploy(PlayerTeam, BattleView.ToSim(g.Value)) == null;
            float size = DeployType == "*" ? 1400f : 700f;
            View.ShowDeployMarker(g, ok, size);
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) { CancelDeploy(); return; }
            if (Input.GetMouseButtonDown(0) && g.HasValue && ok)
            {
                DeployAt(g.Value, DeployType);
                if (!shift || DeployType == "*") CancelDeploy();
            }
        }

        public bool Buy(string shipId) => World.TryBuy(PlayerTeam, shipId);
        public bool CancelBuy(string shipId) => World.CancelBuy(PlayerTeam, shipId);

        // Points the camera at a spot on the play plane, zoomed out to at least the given distance.
        public void LookAt(Vector3 point, float atLeast = 0f)
        {
            pivot = new Vector3(point.x, 0, point.z);
            distance = Mathf.Clamp(Mathf.Max(distance, atLeast), MinDist, MaxDist);
        }

        public void Focus(IEnumerable<Ship> ships)
        {
            var list = ships.Where(s => !s.Dead).ToList();
            if (list.Count > 0) pivot = Centre(list);
        }

        // ---------- input ----------

        void HandleKeys()
        {
            if (Input.GetKeyDown(KeyCode.F5)) Begin(Config);
            if (Input.GetKeyDown(KeyCode.F10)) Application.Quit();
            if (Input.GetKeyDown(KeyCode.P)) Paused = !Paused;
            if (Input.GetKeyDown(KeyCode.M)) SoundBoard.Current?.ToggleMute();
            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus)) GameSpeed = Mathf.Min(4f, GameSpeed * 2f);
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) GameSpeed = Mathf.Max(0.25f, GameSpeed / 2f);
            if (Input.GetKeyDown(KeyCode.H)) hud.ToggleHelp();
            // Escape backs out one step: cancel attack-move or deploying, else clear the selection, else the menu.
            if (Input.GetKeyDown(KeyCode.Escape) && !Deploying && !AttackMoveArmed)
            {
                if (Selected.Count > 0) Select(new Ship[0]);
                else { OpenMenu(); return; }
            }
            if (Input.GetKeyDown(KeyCode.R) && !Deploying) BeginDeploy("*");
            if (Input.GetKeyDown(KeyCode.F)) Focus(Selected);
            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (Input.GetKeyDown(KeyCode.X)) CommandStop();
            if (Input.GetKeyDown(KeyCode.T) && Selected.Count > 0 && !Inspecting && !Deploying) AttackMoveArmed = true;
            if (AttackMoveArmed && (Input.GetKeyDown(KeyCode.Escape) || Selected.Count == 0 || Inspecting)) AttackMoveArmed = false;
            if (ctrl && Input.GetKeyDown(KeyCode.A)) SelectAllOwn();

            for (int n = 1; n <= 9; n++)
            {
                if (!Input.GetKeyDown(KeyCode.Alpha0 + n)) continue;
                if (ctrl) { groups[n] = Selected.ToList(); continue; }
                if (!groups.TryGetValue(n, out var g) || g.Count == 0) continue;
                Select(g);
                GroupRecalls++;
                if (lastGroupKey == n && Time.unscaledTime - lastGroupTime < 0.35f) Focus(g);
                lastGroupKey = n; lastGroupTime = Time.unscaledTime;
            }
        }

        void HandleCamera(float dt)
        {
            float pan = distance * 0.9f * dt;
            var fwd = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
            var right = Quaternion.Euler(0, yaw, 0) * Vector3.right;
            var move = Vector3.zero;
            bool ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (!ctrlHeld) // Ctrl+key are commands, not panning
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) move += fwd;
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) move -= fwd;
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) move += right;
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) move -= right;
            }
            if (Application.isFocused && !dragging)
            {
                var m = Input.mousePosition;
                const float edge = 6f;
                if (m.x >= 0 && m.x <= Screen.width && m.y >= 0 && m.y <= Screen.height)
                {
                    if (m.x < edge) move -= right; else if (m.x > Screen.width - edge) move += right;
                    if (m.y < edge) move -= fwd; else if (m.y > Screen.height - edge) move += fwd;
                }
            }
            pivot += move.normalized * pan;

            if (Input.GetKey(KeyCode.Q)) yaw -= 70f * dt;
            if (Input.GetKey(KeyCode.E)) yaw += 70f * dt;
            if (Input.GetMouseButton(2)) yaw += Input.GetAxisRaw("Mouse X") * 3f;

            float wheel = Input.mouseScrollDelta.y;
            if (Mathf.Abs(wheel) > 0.01f)
            {
                // Zoom toward the point under the cursor.
                var before = GroundUnderMouse();
                distance = Mathf.Clamp(distance * Mathf.Pow(0.85f, wheel), MinDist, MaxDist);
                PlaceCamera();
                var after = GroundUnderMouse();
                if (before.HasValue && after.HasValue) pivot += before.Value - after.Value;
            }
        }

        // 35° tall at 16:9 and wider. Narrower windows (towards 1:1) widen the view so you still see as much across.
        const float BaseFov = 35f;
        static float FovFor(float aspect)
        {
            const float wide = 16f / 9f;
            if (aspect >= wide) return BaseFov;
            float halfTan = Mathf.Tan(BaseFov * 0.5f * Mathf.Deg2Rad) * wide / Mathf.Max(0.3f, aspect);
            return Mathf.Min(75f, 2f * Mathf.Atan(halfTan) * Mathf.Rad2Deg);
        }

        void PlaceCamera()
        {
            cam.fieldOfView = FovFor(cam.aspect);
            // Lower, more cinematic angle close in; steeper overview far out.
            float pitch = Mathf.Lerp(38f, 62f, Mathf.InverseLerp(MinDist, MaxDist, distance));
            var rot = Quaternion.Euler(pitch, yaw, 0);
            transform.SetPositionAndRotation(pivot - rot * Vector3.forward * distance, rot);
        }

        void HandleMouse()
        {
            var mouse = (Vector2)Input.mousePosition;
            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (Deploying)
            {
                if (hud.PointerOverPanel()) View.ShowDeployMarker(null, false, 0);
                else HandleDeploy(mouse, shift);
                return;
            }
            // Over the HUD panels nothing new starts, but a drag already under way still finishes there
            // (letting go over the build bar ends the box as usual).
            bool overPanel = hud.PointerOverPanel();
            if (overPanel && !leftDown) return;
            // The button was let go somewhere we didn't see (another window): end the drag anyway.
            bool released = leftDown && (Input.GetMouseButtonUp(0) || !Input.GetMouseButton(0));

            if (AttackMoveArmed && !overPanel)
            {
                if (Input.GetMouseButtonDown(1)) { AttackMoveArmed = false; return; }
                if (Input.GetMouseButtonDown(0))
                {
                    AttackMoveArmed = false;
                    var target = PickAt(mouse, s => s.Team != PlayerTeam);
                    if (target != null) CommandAttack(target);
                    else { var g = GroundUnderMouse(); if (g.HasValue) CommandMove(g.Value, shift, attack: true); }
                    leftDown = false;
                    return;
                }
                return;
            }

            if (Input.GetMouseButtonDown(0)) { leftDown = true; dragStart = mouse; dragging = false; }
            if (leftDown && (mouse - dragStart).magnitude > 8f) dragging = true;
            if (released)
            {
                leftDown = false;
                if (overPanel && !dragging) { dragging = false; return; } // a click that ended on the panel
                if (dragging)
                {
                    var rect = Rect.MinMaxRect(Mathf.Min(dragStart.x, mouse.x), Mathf.Min(dragStart.y, mouse.y), Mathf.Max(dragStart.x, mouse.x), Mathf.Max(dragStart.y, mouse.y));
                    Select(Own.Where(s => OnScreen(s, out var p) && rect.Contains(p)), shift);
                }
                else
                {
                    // Clicking right on one of your big ships always selects it. Otherwise, anywhere inside a
                    // fight zone (dogfight or strike ring) selects your craft in it, or, if you have none there,
                    // the enemy craft (to look at them).
                    var exact = View.Pick(cam.ScreenPointToRay(mouse));
                    bool onOwnBig = exact != null && exact.Team == PlayerTeam && !Hulls.IsSmall(exact.Hull);
                    var fight = onOwnBig ? null : FightUnderMouse();
                    var strike = onOwnBig || fight != null ? null : StrikeUnderMouse();
                    var zoneCraft = fight != null ? fight.Alive.ToList() : strike.HasValue ? strike.Value.attackers : null;
                    var hit = onOwnBig ? exact : zoneCraft == null ? PickAt(mouse, s => s.Team == PlayerTeam) : null;
                    if (zoneCraft != null && zoneCraft.Any(s => s.Team == PlayerTeam))
                        Select(zoneCraft.Where(s => s.Team == PlayerTeam), shift);
                    else if (zoneCraft != null && zoneCraft.Count > 0)
                        Select(zoneCraft, false, inspect: true);
                    else if (hit != null)
                    {
                        if (hit == lastClicked && Time.unscaledTime - lastClickTime < 0.35f)
                            Select(Own.Where(s => s.Def.id == hit.Def.id && OnScreen(s, out _)), shift);
                        else if (shift && Selected.Contains(hit)) { Selected.Remove(hit); View.SetSelected(hit, false); }
                        else Select(new[] { hit }, shift);
                        lastClicked = hit; lastClickTime = Time.unscaledTime;
                    }
                    else if (!shift) Select(new Ship[0]);
                }
                dragging = false;
            }

            if (Input.GetMouseButtonDown(1) && Selected.Count > 0 && !Inspecting && !overPanel)
            {
                var exact = View.Pick(cam.ScreenPointToRay(mouse));
                var fight = FightUnderMouse();
                var hit = exact != null && exact.Team != PlayerTeam && !Hulls.IsSmall(exact.Hull) ? exact : null;
                var strike = hit == null && fight == null ? StrikeUnderMouse() : null;
                if (hit == null && fight != null && fight.Alive.Any(s => s.Team != PlayerTeam)) CommandFight(fight);
                else if (hit == null && strike.HasValue && strike.Value.big.Team != PlayerTeam) CommandAttack(strike.Value.big);
                else if (hit == null && strike.HasValue && strike.Value.attackers.Any(s => s.Team != PlayerTeam))
                {
                    // Our ship is under attack: go after the craft attacking it (fighters on fighters start a dogfight).
                    var g = GroundUnderMouse().Value;
                    CommandAttack(strike.Value.attackers.Where(s => s.Team != PlayerTeam)
                        .OrderBy(s => (BattleView.ToWorld(s.Pos) - g).sqrMagnitude).First());
                }
                else if ((hit ??= PickAt(mouse, s => s.Team != PlayerTeam)) != null) CommandAttack(hit);
                else
                {
                    var g = GroundUnderMouse();
                    bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                    if (g.HasValue) CommandMove(g.Value, shift, attack: ctrl);
                }
            }
        }

        // Generous click target: an exact hull hit first; otherwise the nearest matching ship whose
        // on-screen footprint (at least ClickRadius pixels) contains the cursor.
        public const float ClickRadius = 30f;

        public Ship PickAt(Vector2 mouse, System.Func<Ship, bool> which)
        {
            var exact = View.Pick(cam.ScreenPointToRay(mouse));
            if (exact != null && which(exact)) return exact;
            Ship best = null;
            float bestD = float.MaxValue;
            foreach (var s in World.Alive)
            {
                if (!which(s)) continue;
                var v = View.ViewOf(s);
                var centre = BattleView.ShownCentre(v);
                var p = cam.WorldToScreenPoint(centre);
                if (p.z <= 0) continue;
                // Half the ship's shown length, measured on screen.
                var edge = cam.WorldToScreenPoint(centre + cam.transform.right * v.Length * 0.5f);
                float radius = Mathf.Max(ClickRadius, Vector2.Distance(p, edge));
                float d = Vector2.Distance(p, mouse);
                if (d <= radius && d < bestD) { best = s; bestD = d; }
            }
            return best;
        }

        public bool DragRect(out Rect rect)
        {
            var m = (Vector2)Input.mousePosition;
            rect = Rect.MinMaxRect(Mathf.Min(dragStart.x, m.x), Mathf.Min(dragStart.y, m.y), Mathf.Max(dragStart.x, m.x), Mathf.Max(dragStart.y, m.y));
            return leftDown && dragging;
        }

        bool OnScreen(Ship s, out Vector2 screen)
        {
            var v = View.ViewOf(s);
            var p = cam.WorldToScreenPoint(BattleView.ShownCentre(v));
            screen = p;
            return p.z > 0 && p.x >= 0 && p.x <= Screen.width && p.y >= 0 && p.y <= Screen.height;
        }

        Vector3? GroundUnderMouse()
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, Vector3.zero);
            return plane.Raycast(ray, out float d) ? ray.GetPoint(d) : (Vector3?)null;
        }

        static Vector3 Centre(List<Ship> ships) => ships.Count == 0 ? Vector3.zero
            : new Vector3((float)ships.Average(s => s.Pos.X), 0, (float)ships.Average(s => s.Pos.Y));

        void OnGUI()
        {
            if (!DragRect(out var r)) return;
            // GUI space has y down.
            var gr = new Rect(r.xMin, Screen.height - r.yMax, r.width, r.height);
            var old = GUI.color;
            GUI.color = new Color(0.5f, 1f, 0.6f, 0.15f);
            GUI.DrawTexture(gr, Texture2D.whiteTexture);
            GUI.color = new Color(0.5f, 1f, 0.6f, 0.8f);
            GUI.DrawTexture(new Rect(gr.xMin, gr.yMin, gr.width, 1), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(gr.xMin, gr.yMax - 1, gr.width, 1), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(gr.xMin, gr.yMin, 1, gr.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(gr.xMax - 1, gr.yMin, 1, gr.height), Texture2D.whiteTexture);
            GUI.color = old;
        }
    }
}
