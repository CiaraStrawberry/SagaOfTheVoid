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

        public World World { get; private set; }
        public BattleView View { get; private set; }
        public readonly List<Ship> Selected = new List<Ship>();
        public float GameSpeed = 1f;
        public bool Paused;

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
        const float MinDist = 1200f, MaxDist = 16000f;

        // mouse selection
        Vector2 dragStart; bool dragging; bool leftDown;
        float lastClickTime; Ship lastClicked;

        void Start()
        {
            cam = GetComponent<Camera>();
            cam.fieldOfView = 35f;
            cam.nearClipPlane = 30f;
            cam.farClipPlane = 60000f;

            var lib = ShipLibrary.Load();
            World = new World(lib.Rules(), seed);
            World.PlayerTeams.Add(PlayerTeam);
            Fleets.Spawn(World, quickFleets ? Fleets.Quick : Fleets.Full, quickFleets ? 2800f : 4200f);

            BattleView.BuildStage(lib, cam);
            View = new BattleView(World, lib, cam);
            hud = new SkirmishHud(this, lib, cam);

            var mine = World.Alive.Where(s => s.Team == PlayerTeam).ToList();
            pivot = Centre(mine) + new Vector3(2000f, 0, 0);
            PlaceCamera();
        }

        void Update()
        {
            float dt = Time.unscaledDeltaTime;
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
        }

        // ---------- commands (public so tests and later UI can drive them) ----------

        public IEnumerable<Ship> Own => World.Alive.Where(s => s.Team == PlayerTeam);

        public void Select(IEnumerable<Ship> ships, bool add = false)
        {
            if (!add) { foreach (var s in Selected) View.SetSelected(s, false); Selected.Clear(); }
            foreach (var s in ships)
                if (!s.Dead && s.Team == PlayerTeam && !Selected.Contains(s)) { Selected.Add(s); View.SetSelected(s, true); }
        }

        public void SelectAllOwn() => Select(Own);

        public void CommandMove(Vector3 worldPoint, bool queue)
        {
            if (Selected.Count == 0) return;
            World.OrderMove(Selected, BattleView.ToSim(worldPoint), queue);
            View.Ping(worldPoint, false);
        }

        public void CommandAttack(Ship target)
        {
            if (Selected.Count == 0 || target == null || target.Team == PlayerTeam) return;
            World.OrderAttack(Selected, target);
            var v = View.ViewOf(target);
            if (v != null) View.Ping(v.Go.transform.position, true, v.Length * 1.3f);
        }

        public void CommandStop() => World.OrderStop(Selected);

        public bool Buy(string shipId) => World.TryBuy(PlayerTeam, shipId);
        public bool CancelBuy(string shipId) => World.CancelBuy(PlayerTeam, shipId);

        public void Focus(IEnumerable<Ship> ships)
        {
            var list = ships.Where(s => !s.Dead).ToList();
            if (list.Count > 0) pivot = Centre(list);
        }

        // ---------- input ----------

        void HandleKeys()
        {
            if (Input.GetKeyDown(KeyCode.F5)) UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            if (Input.GetKeyDown(KeyCode.F10)) Application.Quit();
            if (Input.GetKeyDown(KeyCode.P)) Paused = !Paused;
            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus)) GameSpeed = Mathf.Min(4f, GameSpeed * 2f);
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) GameSpeed = Mathf.Max(0.25f, GameSpeed / 2f);
            if (Input.GetKeyDown(KeyCode.H)) hud.ToggleHelp();
            if (Input.GetKeyDown(KeyCode.Escape)) Select(new Ship[0]);
            if (Input.GetKeyDown(KeyCode.F)) Focus(Selected);
            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (Input.GetKeyDown(KeyCode.X)) CommandStop();
            if (ctrl && Input.GetKeyDown(KeyCode.A)) SelectAllOwn();

            for (int n = 1; n <= 9; n++)
            {
                if (!Input.GetKeyDown(KeyCode.Alpha0 + n)) continue;
                if (ctrl) { groups[n] = Selected.ToList(); continue; }
                if (!groups.TryGetValue(n, out var g) || g.Count == 0) continue;
                Select(g);
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

        void PlaceCamera()
        {
            // Lower, more cinematic angle close in; steeper overview far out.
            float pitch = Mathf.Lerp(38f, 62f, Mathf.InverseLerp(MinDist, MaxDist, distance));
            var rot = Quaternion.Euler(pitch, yaw, 0);
            transform.SetPositionAndRotation(pivot - rot * Vector3.forward * distance, rot);
        }

        void HandleMouse()
        {
            if (hud.PointerOverPanel()) return;
            var mouse = (Vector2)Input.mousePosition;
            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (Input.GetMouseButtonDown(0)) { leftDown = true; dragStart = mouse; dragging = false; }
            if (leftDown && (mouse - dragStart).magnitude > 8f) dragging = true;
            if (leftDown && Input.GetMouseButtonUp(0))
            {
                leftDown = false;
                if (dragging)
                {
                    var rect = Rect.MinMaxRect(Mathf.Min(dragStart.x, mouse.x), Mathf.Min(dragStart.y, mouse.y), Mathf.Max(dragStart.x, mouse.x), Mathf.Max(dragStart.y, mouse.y));
                    Select(Own.Where(s => OnScreen(s, out var p) && rect.Contains(p)), shift);
                }
                else
                {
                    var hit = PickAt(mouse, s => s.Team == PlayerTeam);
                    if (hit != null)
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

            if (Input.GetMouseButtonDown(1) && Selected.Count > 0)
            {
                var hit = PickAt(mouse, s => s.Team != PlayerTeam);
                if (hit != null) CommandAttack(hit);
                else
                {
                    var g = GroundUnderMouse();
                    if (g.HasValue) CommandMove(g.Value, shift);
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
                var centre = v.Go.transform.position + Vector3.up * v.Height;
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
            var p = cam.WorldToScreenPoint(v.Go.transform.position + Vector3.up * v.Height);
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
