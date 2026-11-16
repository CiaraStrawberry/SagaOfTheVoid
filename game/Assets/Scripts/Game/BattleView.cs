using System;
using System.Collections.Generic;
using System.Linq;
using Saga.Sim;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = System.Random;

namespace Saga.Game
{
    // Draws a running World: 3D ship models on the flat play plane, shots, impacts, explosions, health bars.
    // Shared by the playable skirmish and the editor's battle recorder. Never changes the World.
    //
    // Sim plane (X, Y) is world (X, 0, Z). Sim heading turns anticlockwise seen from above; Unity yaw turns clockwise.
    public class BattleView
    {
        public const float SizeK = 30f, SizePow = 0.6f;  // shown length = 30 * modelLength^0.6 (big ships shrink, small grow)
        public const float SquadronBoost = 1.8f;
        public static readonly string[] TeamColour = { "Blue", "Red" };
        public static readonly Color[] TeamTint = { new Color(0.35f, 0.65f, 1f), new Color(1f, 0.38f, 0.3f) };

        public class ShipView
        {
            public Ship Ship;
            public GameObject Go;
            public Transform Model;
            public SpriteRenderer Disc, FieldRing, Select;
            public SpriteRenderer[] Engines;
            public ShipModel Paint;
            public Transform BarBack, BarShield, BarArmor;
            public Collider[] HullColliders;
            public float Length, Height, Bank, Pulse;
            public double LastHeading, ShowBarsUntil;
            public int LastArmor, LastShield;
            public bool Selected;
        }

        class Fx
        {
            public GameObject Go;
            public LineRenderer Line;
            public SpriteRenderer Sprite;
            public float Age, Life, Size, Yaw;
            public Vector3 From, To, Velocity;
            public string Kind;
            public Color Colour;
            public bool Flat;
            public Transform Anchor;   // shot end point follows this ship part...
            public Vector3 Local;      // ...at this local point
        }

        // Where a shot will land, decided when it is fired, kept so the impact appears at the same spot.
        class ShotPlan { public Transform Anchor; public Vector3 Local; public bool OnShield; public Vector3 Dir; }

        public readonly World World;
        public readonly Camera Cam;
        readonly ShipLibrary lib;
        readonly Transform root;
        readonly Dictionary<Ship, ShipView> views = new Dictionary<Ship, ShipView>();
        readonly Dictionary<int, ShotPlan> plans = new Dictionary<int, ShotPlan>();
        readonly List<Fx> fx = new List<Fx>();
        readonly Random rng = new Random(7);
        readonly MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        Sprite dot, ring, glow, white, arc;
        Material addMat, alphaMat;

        public int HullHits, HullFallbacks, ShieldHits;

        public BattleView(World world, ShipLibrary library, Camera cam)
        {
            World = world; lib = library; Cam = cam;
            root = new GameObject("Battle").transform;
            MakeShared();
            foreach (var s in world.Ships) GetView(s);
        }

        public IEnumerable<ShipView> Views => views.Values;
        public ShipView ViewOf(Ship s) => views.TryGetValue(s, out var v) ? v : null;

        public static Vector3 ToWorld(Vec2 p) => new Vector3((float)p.X, 0, (float)p.Y);
        public static Vec2 ToSim(Vector3 p) => new Vec2(p.x, p.z);

        // ---------- stage: sky, lights, post-processing ----------

        public static void BuildStage(ShipLibrary lib, Camera cam)
        {
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.allowHDR = true;
            var camData = cam.GetComponent<UniversalAdditionalCameraData>() ?? cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
            camData.renderPostProcessing = true;
            camData.antialiasing = AntialiasingMode.None;

            RenderSettings.skybox = lib.skybox;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.5f, 0.55f, 0.65f);
            RenderSettings.ambientEquatorColor = new Color(0.35f, 0.36f, 0.4f);
            RenderSettings.ambientGroundColor = new Color(0.2f, 0.18f, 0.18f);

            var sun = new GameObject("Sun").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.95f, 0.86f);
            sun.intensity = 2.4f;
            sun.shadows = LightShadows.None;
            sun.transform.rotation = Quaternion.Euler(60f, -25f, 0f);
            var rim = new GameObject("Rim light").AddComponent<Light>();
            rim.type = LightType.Directional;
            rim.color = new Color(0.45f, 0.6f, 1f);
            rim.intensity = 0.8f;
            rim.transform.rotation = Quaternion.Euler(30f, 160f, 0f);

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            var bloom = profile.Add<Bloom>(true);
            bloom.intensity.Override(0.6f);
            bloom.threshold.Override(1.1f);
            bloom.scatter.Override(0.6f);
            profile.Add<Tonemapping>(true).mode.Override(TonemappingMode.Neutral); // ACES bleached bright effects to white
            profile.Add<Vignette>(true).intensity.Override(0.25f);
            var colour = profile.Add<ColorAdjustments>(true);
            colour.postExposure.Override(0.35f);
            colour.contrast.Override(8f);
            colour.saturation.Override(10f);
            var vol = new GameObject("Post-processing").AddComponent<Volume>();
            vol.isGlobal = true;
            vol.sharedProfile = profile;
        }

        // ---------- per frame ----------

        // Call once per drawn frame with everything the World reported since the last call.
        public void Sync(IList<ShotEvent> shots, IList<ImpactEvent> impacts, IList<Ship> deaths, float dt, double now)
        {
            foreach (var s in World.Ships) if (!s.Dead) UpdateShipView(GetView(s), dt);
            Physics.SyncTransforms();
            foreach (var s in shots) SpawnShot(s);
            foreach (var im in impacts) SpawnImpact(im);
            foreach (var d in deaths)
            {
                var dv = ViewOf(d);
                if (dv == null) continue;
                SpawnExplosion(dv);
                dv.Go.SetActive(false);
                foreach (var b in new[] { dv.BarBack, dv.BarArmor, dv.BarShield }) if (b != null) b.gameObject.SetActive(false);
            }
            foreach (var v in views.Values) if (!v.Ship.Dead) PlaceBars(v, now);
            UpdateFx(dt);
        }

        public void SetSelected(Ship s, bool on)
        {
            var v = ViewOf(s);
            if (v == null) return;
            v.Selected = on;
            v.Select.gameObject.SetActive(on);
        }

        // Brief ring on the play plane where an order was given: green for move, red for attack.
        public void Ping(Vector3 at, bool attack, float size = 260f)
        {
            var c = attack ? new Color(1f, 0.35f, 0.3f, 0.8f) : new Color(0.45f, 1f, 0.55f, 0.7f);
            AddSprite("ring", ring, new Vector3(at.x, 5f, at.z), c, size, 0.45f, Vector3.zero, true);
        }

        // The live ship whose hull a screen ray hits first, if any.
        public Ship Pick(Ray ray)
        {
            Ship best = null;
            float bestDist = float.MaxValue;
            foreach (var v in views.Values)
            {
                if (v.Ship.Dead) continue;
                foreach (var c in v.HullColliders)
                    if (c.Raycast(ray, out var hit, 1e6f) && hit.distance < bestDist) { bestDist = hit.distance; best = v.Ship; }
            }
            return best;
        }

        // ---------- ships ----------

        ShipView GetView(Ship s)
        {
            if (views.TryGetValue(s, out var have)) return have;
            var prefab = lib.Prefab(s.Def.id);
            if (prefab == null) throw new Exception("no prefab for ship " + s.Def.id);
            var v = new ShipView { Ship = s, LastHeading = s.Heading };
            v.Go = new GameObject(s.Def.id + "_" + s.Id);
            v.Go.transform.SetParent(root, false);
            var model = UnityEngine.Object.Instantiate(prefab, v.Go.transform);
            v.Model = model.transform;
            v.Paint = model.GetComponent<ShipModel>();
            v.Paint.ApplyPaint(TeamColour[s.Team % TeamColour.Length]);
            bool small = Hulls.IsSmall(s.Hull);
            v.Length = SizeK * Mathf.Pow(v.Paint.length, SizePow) * (small ? SquadronBoost : 1f);
            model.transform.localScale = Vector3.one * (v.Length / v.Paint.length);
            // Gameplay is flat; heights only separate the layers visually. Fighters fly above the big ships.
            v.Height = small ? 220f : s.Hull == Hull.Frigate ? 40f : 0f;
            var tint = TeamTint[s.Team % TeamTint.Length];

            v.Disc = FlatSprite("team", glow, v.Go.transform, v.Length * (small ? 0.75f : 1.0f), 0f);

            v.Select = FlatSprite("selected", ring, v.Go.transform, v.Length * 1.25f, v.Height);
            Tint(v.Select, new Color(0.55f, 0.95f, 0.6f, 0.55f));
            v.Select.gameObject.SetActive(false);

            // Hull colliders, used to land shots on the surface and to pick ships with the mouse.
            v.HullColliders = v.Paint.renderers.Select(r =>
            {
                var mc = r.gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = r.GetComponent<MeshFilter>().sharedMesh;
                return (Collider)mc;
            }).ToArray();

            // Engine glow at the stern of each hull part (each craft, for squadrons).
            var hulls = small ? v.Paint.renderers : new[] { v.Paint.renderers.OrderByDescending(r => r.bounds.size.sqrMagnitude).First() };
            v.Engines = hulls.Select(r =>
            {
                var b = r.GetComponent<MeshFilter>().sharedMesh.bounds;
                var e = new GameObject("engine").AddComponent<SpriteRenderer>();
                e.sprite = glow; e.sharedMaterial = addMat;
                e.transform.SetParent(r.transform, false);
                e.transform.localPosition = new Vector3(b.center.x, b.center.y, b.min.z);
                return e;
            }).ToArray();

            var field = s.ShieldField;
            if (field != null)
            {
                v.FieldRing = FlatSprite("shield field", ring, v.Go.transform, field.Weapon.aura_radius * 2f / 0.85f, 20f); // ring peaks at 85% of half-size
            }

            if (!small) { v.BarBack = Bar(); v.BarArmor = Bar(); v.BarShield = Bar(); }
            v.LastArmor = s.Armor; v.LastShield = s.Shield;
            v.ShowBarsUntil = -1;
            views[s] = v;
            UpdateShipView(v, 0);
            Tint(v.Disc, new Color(tint.r, tint.g, tint.b, 0.25f));
            return v;
        }

        SpriteRenderer FlatSprite(string name, Sprite sp, Transform parent, float size, float height)
        {
            var r = new GameObject(name).AddComponent<SpriteRenderer>();
            r.sprite = sp; r.sharedMaterial = addMat;
            r.transform.SetParent(parent, false);
            r.transform.localPosition = Vector3.up * height;
            r.transform.localRotation = Quaternion.Euler(90, 0, 0);
            r.transform.localScale = Vector3.one * size;
            return r;
        }

        Transform Bar()
        {
            var r = new GameObject("bar").AddComponent<SpriteRenderer>();
            r.sprite = white; r.sharedMaterial = alphaMat;
            r.transform.SetParent(root, false);
            return r.transform;
        }

        void UpdateShipView(ShipView v, float dt)
        {
            var s = v.Ship;
            v.Go.transform.position = ToWorld(s.Pos);
            // Bank into turns a little, for life; capped, eased.
            if (dt > 0)
            {
                double turn = Math.IEEERemainder(s.Heading - v.LastHeading, 2 * Math.PI) / dt;
                v.Bank = Mathf.Lerp(v.Bank, Mathf.Clamp((float)turn * 25f, -30f, 30f), 0.1f);
            }
            v.LastHeading = s.Heading;
            v.Model.localPosition = new Vector3(0, v.Height, 0);
            v.Model.rotation = Quaternion.Euler(0, -(float)(s.Heading * Mathf.Rad2Deg), 0) * Quaternion.Euler(0, 0, v.Bank);
            bool small = Hulls.IsSmall(s.Hull);
            int craft = World.CraftLeft(s);
            // Squadron craft blink out as the squadron is whittled down.
            if (small)
                for (int i = 0; i < v.Paint.renderers.Length; i++) v.Paint.renderers[i].enabled = i < craft;

            var tint = TeamTint[s.Team % TeamTint.Length];
            Tint(v.Disc, new Color(tint.r, tint.g, tint.b, (small ? 0.22f : 0.25f) * (0.6f + 0.4f * craft / 5f)));

            // Engines: brighter with speed, flickering slightly; always billboarded.
            v.Pulse += dt;
            float throttle = Mathf.Clamp01((float)(s.Speed / Math.Max(1.0, s.Def.speed)));
            for (int i = 0; i < v.Engines.Length; i++)
            {
                var e = v.Engines[i];
                e.enabled = !small || i < craft;
                float flicker = 0.85f + 0.15f * Mathf.Sin(v.Pulse * 37f + i * 1.7f);
                float size = v.Length * (small ? 0.09f : 0.12f) * (0.6f + 0.6f * throttle) / Mathf.Max(0.0001f, e.transform.lossyScale.x / e.transform.localScale.x);
                e.transform.localScale = Vector3.one * size;
                e.transform.rotation = Cam.transform.rotation;
                Tint(e, Hdr(Color.Lerp(new Color(0.55f, 0.75f, 1f), tint, 0.25f), (0.6f + 1.4f * throttle) * flicker));
            }

            if (v.FieldRing != null)
                Tint(v.FieldRing, new Color(0.35f, 0.65f, 1f, 0.035f + 0.025f * Mathf.Sin(v.Pulse * 2.2f)));
        }

        // Homeworld-style: a thin line over the ship while it is taking damage (or selected), fading out after.
        // Armour line runs green -> yellow -> red; a thinner shield line sits on top while shields hold.
        const float BarShowSeconds = 3f, BarFadeSeconds = 1f;

        void PlaceBars(ShipView v, double now)
        {
            if (v.BarBack == null) return;
            var s = v.Ship;
            if (s.Armor != v.LastArmor || s.Shield != v.LastShield)
            {
                v.LastArmor = s.Armor; v.LastShield = s.Shield;
                v.ShowBarsUntil = now + BarShowSeconds;
            }
            float vis = v.Selected ? 1f : Mathf.Clamp01((float)(v.ShowBarsUntil - now) / BarFadeSeconds);
            foreach (var b in new[] { v.BarBack, v.BarArmor, v.BarShield }) b.gameObject.SetActive(vis > 0);
            if (vis <= 0) return;

            float w = Mathf.Max(160f, v.Length * 0.55f), h = 9f, hs = 5f;
            var up = Cam.transform.up;
            var right = Cam.transform.right;
            var basePos = v.Go.transform.position + Vector3.up * v.Height + up * (v.Length * 0.45f + 60f);
            var rot = Cam.transform.rotation;
            float a = Mathf.Clamp01((float)s.Armor / s.StartArmor), sh = Mathf.Clamp01((float)s.Shield / s.StartShield);
            var armourColour = a > 0.5f ? Color.Lerp(new Color(1f, 0.85f, 0.2f), new Color(0.35f, 0.95f, 0.45f), (a - 0.5f) * 2f)
                                        : Color.Lerp(new Color(1f, 0.25f, 0.2f), new Color(1f, 0.85f, 0.2f), a * 2f);
            Set(v.BarBack, basePos, rot, w, h);
            Tint(v.BarBack.GetComponent<Renderer>(), new Color(0f, 0f, 0f, 0.35f * vis));
            Set(v.BarArmor, basePos + right * (-w / 2 + w * a / 2), rot, w * a, h);
            Tint(v.BarArmor.GetComponent<Renderer>(), new Color(armourColour.r, armourColour.g, armourColour.b, 0.9f * vis));
            v.BarShield.gameObject.SetActive(sh > 0);
            Set(v.BarShield, basePos + right * (-w / 2 + w * sh / 2) + up * (h / 2 + hs / 2 + 3f), rot, w * sh, hs);
            Tint(v.BarShield.GetComponent<Renderer>(), new Color(0.45f, 0.75f, 1f, 0.85f * vis));
        }

        static void Set(Transform t, Vector3 p, Quaternion r, float w, float h)
        {
            t.position = p; t.rotation = r; t.localScale = new Vector3(Mathf.Max(0.01f, w), h, 1);
        }

        // ---------- shots ----------

        static Vector3 Centre(ShipView v) => v.Go.transform.position + Vector3.up * (v.Height + 10f);

        Vector3 Muzzle(ShipView v)
        {
            float r = v.Length * 0.3f;
            return Centre(v) + new Vector3((float)(rng.NextDouble() - 0.5) * r, 0, (float)(rng.NextDouble() - 0.5) * r);
        }

        Vector3 MissPoint(ShipView target)
        {
            var d = UnityEngine.Random.insideUnitCircle.normalized * (target.Length * 0.7f + 180f);
            return Centre(target) + new Vector3(d.x, 0, d.y);
        }

        static Vector3 FlatDir(Vector3 from, Vector3 to)
        {
            var d = to - from; d.y = 0;
            return d.sqrMagnitude < 1 ? Vector3.forward : d.normalized;
        }

        // Traces from the muzzle at a random spot on the target's footprint and returns where it strikes the hull.
        bool HullPoint(ShipView target, Vector3 muzzle, out Vector3 point, out Transform part)
        {
            point = default; part = null;
            float r = target.Length * 0.35f;
            for (int attempt = 0; attempt < 3; attempt++)
            {
                var aim = Centre(target) + (attempt < 2
                    ? new Vector3((float)(rng.NextDouble() - 0.5) * 2 * r, 0, (float)(rng.NextDouble() - 0.5) * 2 * r)
                    : Vector3.zero);
                var dir = aim - muzzle;
                var ray = new Ray(muzzle, dir.normalized);
                float best = float.MaxValue;
                foreach (var c in target.HullColliders)
                {
                    var rend = c.GetComponent<Renderer>();
                    if (rend != null && !rend.enabled) continue; // lost squadron craft
                    if (c.Raycast(ray, out var hit, dir.magnitude * 2f) && hit.distance < best)
                    {
                        best = hit.distance;
                        point = hit.point + hit.normal * 2f;
                        part = c.transform;
                    }
                }
                if (part != null) return true;
            }
            return false;
        }

        ShotPlan PlanShot(ShipView target, Vector3 muzzle, bool onShield)
        {
            var dir = FlatDir(target.Go.transform.position, muzzle);
            if (onShield)
            {
                // Shield bubble: the point on its rim facing the shooter, a little jittered.
                ShieldHits++;
                var jitter = Quaternion.Euler(0, (float)(rng.NextDouble() - 0.5) * 50f, 0) * dir;
                var p = Centre(target) + jitter * target.Length * 0.6f;
                return new ShotPlan { Anchor = target.Go.transform, Local = target.Go.transform.InverseTransformPoint(p), OnShield = true, Dir = dir };
            }
            if (HullPoint(target, muzzle, out var hitPoint, out var part))
            {
                HullHits++;
                return new ShotPlan { Anchor = part, Local = part.InverseTransformPoint(hitPoint), Dir = dir };
            }
            HullFallbacks++;
            return new ShotPlan { Anchor = target.Go.transform, Local = Vector3.up * (target.Height + 10f), Dir = dir };
        }

        static Vector3 Where(ShotPlan p) => p.Anchor.TransformPoint(p.Local);

        void SpawnShot(ShotEvent e)
        {
            var from = ViewOf(e.From); var to = ViewOf(e.To);
            if (from == null || to == null) return;
            var a = Muzzle(from);
            ShotPlan plan = null;
            Vector3 b;
            if (e.Hit)
            {
                plan = PlanShot(to, a, e.AtShield && e.Visual != "repair_beam");
                plans[e.ShotId] = plan;
                b = Where(plan);
            }
            else b = MissPoint(to);
            var c = TeamTint[e.From.Team % TeamTint.Length];
            Fx f;
            switch (e.Visual)
            {
                case "beam": f = AddLine("beam", a, b, Hdr(Color.Lerp(c, Color.white, 0.4f), 2.2f), 30f, 0.6f); break;
                case "repair_beam": f = AddLine("beam", a, b, Hdr(new Color(0.4f, 1f, 0.6f), 1.6f), 12f, 0.5f); break;
                case "missile": f = AddLine("missile", a, b, Hdr(new Color(1f, 0.85f, 0.6f), 2f), 12f, (float)e.TravelTime, 8); break;
                case "shell": f = AddLine("shell", a, b, Hdr(new Color(1f, 0.65f, 0.25f), 2.0f), 10f, (float)e.TravelTime); break;
                case "bomb": f = AddLine("shell", a, b, Hdr(new Color(0.8f, 1f, 0.35f), 2f), 10f, (float)e.TravelTime); break;
                default: f = AddLine("tracer", a, b, Hdr(Color.Lerp(c, Color.white, 0.5f), 2f), 7f, Mathf.Min(0.6f, (float)e.TravelTime)); break;
            }
            // Hits follow the moving target so they land on the spot they were aimed at.
            if (plan != null) { f.Anchor = plan.Anchor; f.Local = plan.Local; }
        }

        Fx AddLine(string kind, Vector3 a, Vector3 b, Color c, float width, float life, int points = 2)
        {
            var go = new GameObject(kind);
            go.transform.SetParent(root, false);
            var lr = go.AddComponent<LineRenderer>();
            lr.sharedMaterial = addMat;
            lr.positionCount = points;
            lr.useWorldSpace = true;
            lr.startWidth = kind == "missile" ? width * 0.2f : width;
            lr.endWidth = width;
            lr.numCapVertices = 2;
            lr.shadowCastingMode = ShadowCastingMode.Off;
            for (int i = 0; i < points; i++) lr.SetPosition(i, a);
            var f = new Fx { Go = go, Line = lr, Kind = kind, From = a, To = b, Life = Mathf.Max(0.05f, life), Colour = c, Size = width };
            fx.Add(f);
            return f;
        }

        // Missile flight: straight up out of the launch cells first, then a curve over and down onto the target.
        static Vector3 MissilePath(Vector3 from, Vector3 to, float t)
        {
            float dist = Vector3.Distance(from, to);
            float climb = Mathf.Clamp(dist * 0.18f, 220f, 650f);
            var p1 = from + Vector3.up * climb * 1.6f;         // pulls the start vertical
            var p2 = to + Vector3.up * climb * 0.6f + (from - to).normalized * dist * 0.15f;
            float u = 1f - t;
            return u * u * u * from + 3f * u * u * t * p1 + 3f * u * t * t * p2 + t * t * t * to;
        }

        // Launch is slow, then the motor kicks in.
        static float MissileProgress(float t) => t < 0.15f ? t * t / 0.15f * 0.5f : 0.075f + (t - 0.15f) / 0.85f * 0.925f;

        // ---------- impacts and explosions ----------

        void SpawnImpact(ImpactEvent e)
        {
            var v = ViewOf(e.Target);
            if (v == null) return;
            bool heavy = e.Visual == "beam" || e.Visual == "shell" || e.Visual == "missile" || e.Visual == "bomb";
            plans.TryGetValue(e.ShotId, out var plan);
            plans.Remove(e.ShotId);
            var sv = e.Source != null ? ViewOf(e.Source) : null;
            var src = sv != null ? Centre(sv) : Centre(v) + Vector3.forward * 1000f;
            // Shields may have dropped (or come back) while the shot was in flight: re-plan if so.
            if (plan == null || (!e.Repair && plan.OnShield != e.OnShield)) plan = PlanShot(v, src, e.OnShield && !e.Repair);
            var at = Where(plan);
            float yaw = Mathf.Atan2(plan.Dir.x, plan.Dir.z) * Mathf.Rad2Deg;

            if (e.Repair)
            {
                AddSprite("arc", arc, Centre(v), new Color(0.35f, 1f, 0.55f, 0.35f), v.Length * 1.25f, 0.45f, Vector3.zero, true, yaw);
                return;
            }
            if (e.OnShield)
            {
                // A short arc of the shield bubble lights up on the side facing the shooter, with a spark where it struck.
                AddSprite("arc", arc, Centre(v), new Color(0.35f, 0.65f, 1f, heavy ? 0.6f : 0.3f), v.Length * 1.25f, heavy ? 0.3f : 0.18f, Vector3.zero, true, yaw);
                AddSprite("flash", glow, at, Hdr(new Color(0.5f, 0.75f, 1f), 1.2f), heavy ? 55f : 28f, 0.12f, Vector3.zero, false);
                return;
            }
            // Hull hit: a flash on the plating, and for heavy hits a few sparks thrown back toward the shooter.
            AddSprite("flash", glow, at, Hdr(new Color(1f, 0.6f, 0.25f), 1.4f), heavy ? 70f : 28f, heavy ? 0.3f : 0.12f, Vector3.zero, false);
            if (heavy)
                for (int i = 0; i < 4; i++)
                {
                    var d = (plan.Dir + UnityEngine.Random.insideUnitSphere * 0.8f).normalized;
                    AddSprite("debris", dot, at, Hdr(new Color(1f, 0.6f, 0.25f), 1.4f), UnityEngine.Random.Range(6f, 12f), UnityEngine.Random.Range(0.3f, 0.6f), d * UnityEngine.Random.Range(40f, 110f), false);
                }
        }

        void SpawnExplosion(ShipView v)
        {
            var p = Centre(v);
            float size = Hulls.IsSmall(v.Ship.Hull) ? 300f : v.Length * 1.7f;
            AddSprite("flash", glow, p, Hdr(new Color(1f, 0.6f, 0.25f), 1.5f), size, 1.0f, Vector3.zero, false);
            AddSprite("ring", ring, p, new Color(1f, 0.45f, 0.15f, 0.5f), size * 1.6f, 1.2f, Vector3.zero, true);
            int n = Hulls.IsSmall(v.Ship.Hull) ? 6 : 22;
            for (int i = 0; i < n; i++)
            {
                var d = UnityEngine.Random.onUnitSphere; d.y = Mathf.Abs(d.y) * 0.6f;
                AddSprite("debris", dot, p, Hdr(new Color(1f, 0.55f, 0.2f), 1.4f), UnityEngine.Random.Range(14f, 36f),
                    UnityEngine.Random.Range(0.8f, 1.8f), d * UnityEngine.Random.Range(0.3f, 1f) * size, false);
            }
        }

        void AddSprite(string kind, Sprite sp, Vector3 p, Color c, float size, float life, Vector3 vel, bool flat, float yaw = 0f)
        {
            var go = new GameObject(kind);
            go.transform.SetParent(root, false);
            var r = go.AddComponent<SpriteRenderer>();
            r.sprite = sp; r.sharedMaterial = addMat; Tint(r, c);
            go.transform.position = p;
            fx.Add(new Fx { Go = go, Sprite = r, Kind = kind, Life = life, Colour = c, Size = size, From = p, Velocity = vel, Flat = flat, Yaw = yaw });
        }

        void UpdateFx(float dt)
        {
            for (int i = fx.Count - 1; i >= 0; i--)
            {
                var f = fx[i];
                f.Age += dt;
                float t = Mathf.Clamp01(f.Age / f.Life);
                if (f.Age > f.Life + 0.02f)
                {
                    Kill(f.Go);
                    fx.RemoveAt(i);
                    continue;
                }
                if (f.Line != null)
                {
                    if (f.Anchor != null && f.Anchor.gameObject.activeInHierarchy) f.To = f.Anchor.TransformPoint(f.Local);
                    if (f.Kind == "beam")
                    {
                        f.Line.SetPosition(0, f.From); f.Line.SetPosition(1, f.To);
                        f.Line.widthMultiplier = 1f - t * 0.7f;
                        f.Line.startColor = f.Line.endColor = f.Colour * (1f - t);
                    }
                    else if (f.Kind == "missile")
                    {
                        // A short curved smoke-and-flame trail behind the missile along its path.
                        float head = MissileProgress(t), tail = Mathf.Max(0f, head - 0.05f);
                        int n = f.Line.positionCount;
                        for (int k = 0; k < n; k++) f.Line.SetPosition(k, MissilePath(f.From, f.To, Mathf.Lerp(tail, head, k / (float)(n - 1))));
                        f.Line.startColor = new Color(0, 0, 0, 0);
                        f.Line.endColor = f.Colour;
                    }
                    else
                    {
                        var head = Vector3.Lerp(f.From, f.To, t);
                        float tail = f.Kind == "tracer" ? 60f : 24f; // short streaks, not long lines
                        var back = head - (f.To - f.From).normalized * tail;
                        f.Line.SetPosition(0, back); f.Line.SetPosition(1, head);
                        f.Line.startColor = f.Colour * 0.6f;
                        f.Line.endColor = f.Colour;
                    }
                    continue;
                }
                float grow = f.Kind == "arc" ? 1f : f.Kind == "ring" ? Mathf.Lerp(0.5f, 1.15f, Mathf.Sqrt(t)) : f.Kind == "flash" ? Mathf.Lerp(0.5f, 1.2f, Mathf.Sqrt(t)) : 1f - t * 0.5f;
                f.Go.transform.localScale = Vector3.one * f.Size * grow;
                f.Go.transform.position = f.From + f.Velocity * Mathf.Sqrt(t);
                f.Go.transform.rotation = f.Flat ? Quaternion.Euler(0, f.Yaw, 0) * Quaternion.Euler(90, 0, 0) : Cam.transform.rotation;
                var c = f.Colour; c.a *= (1f - t) * (1f - t);
                Tint(f.Sprite, c);
            }
        }

        // ---------- shared bits ----------

        void MakeShared()
        {
            white = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
            dot = Sprite.Create(Radial(64, r => r < 0.5f ? 1 : Mathf.Clamp01((1 - r) * 2)), new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
            glow = Sprite.Create(Radial(128, r => Mathf.Pow(Mathf.Clamp01(1 - r), 2.2f)), new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f), 128);
            ring = Sprite.Create(Radial(256, r => Mathf.Exp(-Mathf.Pow((r - 0.85f) / 0.06f, 2)) + 0.12f * Mathf.Clamp01(1 - r)), new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f), 256);
            arc = Sprite.Create(Arc(256), new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f), 256);
            if (lib.fxShader == null) throw new Exception("ShipLibrary has no fx shader");
            addMat = new Material(lib.fxShader);
            addMat.SetFloat("_SrcBlend", (float)BlendMode.One);
            addMat.SetFloat("_DstBlend", (float)BlendMode.One);
            alphaMat = new Material(lib.fxShader);
            alphaMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            alphaMat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            alphaMat.renderQueue = 3100;
        }

        static Texture2D Radial(int n, Func<float, float> f)
        {
            var t = new Texture2D(n, n, TextureFormat.RGBA32, true) { wrapMode = TextureWrapMode.Clamp };
            var px = new Color[n * n];
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float dx = (x + 0.5f) / n * 2 - 1, dy = (y + 0.5f) / n * 2 - 1;
                    px[y * n + x] = new Color(1, 1, 1, Mathf.Clamp01(f(Mathf.Sqrt(dx * dx + dy * dy))));
                }
            t.SetPixels(px); t.Apply(true);
            return t;
        }

        // A thin band of the shield bubble, brightest straight ahead (+y), fading out about 70 degrees either side.
        static Texture2D Arc(int n)
        {
            var t = new Texture2D(n, n, TextureFormat.RGBA32, true) { wrapMode = TextureWrapMode.Clamp };
            var px = new Color[n * n];
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float dx = (x + 0.5f) / n * 2 - 1, dy = (y + 0.5f) / n * 2 - 1;
                    float r = Mathf.Sqrt(dx * dx + dy * dy);
                    float ang = Mathf.Atan2(dx, dy);
                    float band = Mathf.Exp(-Mathf.Pow((r - 0.86f) / 0.045f, 2)) + 0.25f * Mathf.Exp(-Mathf.Pow((r - 0.8f) / 0.12f, 2));
                    float spread = Mathf.Pow(Mathf.Clamp01(Mathf.Cos(ang)), 3f);
                    px[y * n + x] = new Color(1, 1, 1, Mathf.Clamp01(band * spread));
                }
            t.SetPixels(px); t.Apply(true);
            return t;
        }

        // Effect colours go in through the material's _Color, per object (sprite vertex tint did not reach the shader).
        void Tint(Renderer r, Color c)
        {
            mpb.Clear();
            mpb.SetColor("_Color", c);
            r.SetPropertyBlock(mpb);
        }

        // The recorder runs outside Play mode, where Destroy is not allowed.
        static void Kill(GameObject go)
        {
            if (Application.isPlaying) UnityEngine.Object.Destroy(go);
            else UnityEngine.Object.DestroyImmediate(go);
        }

        // Brightens a colour for bloom without touching its alpha (Color * k scales alpha too).
        public static Color Hdr(Color c, float k) => new Color(c.r * k, c.g * k, c.b * k, c.a);
    }
}
