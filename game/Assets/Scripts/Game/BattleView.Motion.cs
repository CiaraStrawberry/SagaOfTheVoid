using System;
using System.Collections.Generic;
using System.Linq;
using Saga.Sim;
using UnityEngine;

namespace Saga.Game
{
    // Two pieces of pure animation (the battle rules don't change):
    //   Warp-in   ships arrive through a warp portal, shooting in fast and braking hard (2017: Forge3D WarpJump,
    //             with its own textures). Used for starting fleets and for ships deployed from the reserve.
    //   Craft     fighter squadrons are one unit in the rules; here each craft flies on its own: in formation
    //             when cruising, weaving in a dogfight, and circling individually when attacking a big ship.
    public partial class BattleView
    {
        // ---------- warp-in ----------

        const float WarpOpen = 0.35f, WarpFly = 0.9f, WarpFade = 0.7f;

        public class Warp
        {
            public float Age, Delay;
            public Vector3 Dir;                 // the ship's heading when it arrives
            public SpriteRenderer Portal, Aura;
            public LineRenderer[] Streaks;
            public bool Flashed;
            public float Total => Delay + WarpOpen + WarpFly + WarpFade;
        }

        Sprite warpTunnel, warpRing, warpAura, warpSpark;

        void MakeWarpSprites()
        {
            Sprite S(Texture2D t, Sprite fallback) => t == null ? fallback : Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f), t.width);
            warpTunnel = S(lib.warpTunnel, glow);
            warpRing = S(lib.warpRing, ring);
            warpAura = S(lib.warpAura, glow);
            warpSpark = S(lib.warpSpark, dot);
        }

        void StartWarp(ShipView v, float delay)
        {
            var dir = Quaternion.Euler(0, -(float)(v.Ship.Heading * Mathf.Rad2Deg), 0) * Vector3.forward;
            var w = new Warp { Delay = delay, Dir = dir };
            var tint = TeamTint[v.Ship.Team % TeamTint.Length];
            float size = Hulls.IsSmall(v.Ship.Hull) ? v.Length * 1.1f : v.Length * 1.3f;

            // The portal stands across the line of arrival, turning, in team-tinted light.
            w.Portal = new GameObject("warp portal").AddComponent<SpriteRenderer>();
            w.Portal.sprite = warpTunnel; w.Portal.sharedMaterial = addMat;
            // Fixed in the world where the ship arrives (a squadron's own point keeps moving while its craft
            // are still flying straight out of the jump).
            var portalAt = v.Go.transform.position + Vector3.up * v.Height;
            w.Portal.transform.SetParent(root, false);
            w.Portal.transform.position = portalAt;
            w.Portal.transform.localScale = Vector3.one * 0.01f;
            w.Aura = new GameObject("warp aura").AddComponent<SpriteRenderer>();
            w.Aura.sprite = warpAura; w.Aura.sharedMaterial = addMat;
            w.Aura.transform.SetParent(root, false);
            w.Aura.transform.position = portalAt;
            w.Aura.transform.localScale = Vector3.one * size * 1.6f;
            Tint(w.Portal, new Color(0, 0, 0, 0)); Tint(w.Aura, new Color(0, 0, 0, 0));

            int lanes = v.Crafts != null ? v.Crafts.Length : 1;
            w.Streaks = new LineRenderer[lanes];
            for (int i = 0; i < lanes; i++)
            {
                var lr = new GameObject("warp streak").AddComponent<LineRenderer>();
                lr.transform.SetParent(root, false);
                lr.sharedMaterial = addMat;
                lr.positionCount = 2;
                lr.useWorldSpace = true;
                lr.numCapVertices = 3;
                lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lr.enabled = false;
                w.Streaks[i] = lr;
            }
            v.Warp = w;
            if (v.Crafts != null)
            {
                // Fighters come out of the jump in a straight line and keep flying straight ahead for 1 s.
                v.StraightFrom = v.Pulse + delay + WarpOpen + WarpFly;
                v.StraightUntil = v.StraightFrom + 1f + 0.07f * v.Crafts.Length;
                v.StraightAnchor = v.Go.transform.position + Vector3.up * v.Height;
                v.StraightDir = w.Dir;
            }
            if (v.Ship.Arrived >= 0) SoundBoard.Current?.Warp(Centre(v));
            ApplyWarp(v, 0f);
        }

        // Distance still to travel at flight progress p (0..1): starts far out, brakes hard at the end.
        static float WarpDistance(float maxDist, float p) => maxDist * Mathf.Pow(1f - Mathf.Clamp01(p), 3f);

        void ApplyWarp(ShipView v, float dt)
        {
            var w = v.Warp;
            if (w == null) return;
            w.Age += dt;
            float t = w.Age - w.Delay;
            var tint = TeamTint[v.Ship.Team % TeamTint.Length];
            var light = Color.Lerp(tint, Color.white, 0.45f);
            bool small = v.Crafts != null;
            float size = small ? v.Length * 1.1f : v.Length * 1.3f;
            float maxDist = small ? 5000f : 8000f;

            // Portal: opens, holds while the ship comes through, then closes.
            float open = Mathf.Clamp01(t / WarpOpen);
            float close = Mathf.Clamp01((t - WarpOpen - WarpFly) / WarpFade);
            float portalA = t < 0 ? 0 : open * (1f - close);
            // Across the line of arrival, turned halfway toward the camera so it reads as a disc, not a sliver.
            var face = Vector3.Slerp(w.Dir, -Cam.transform.forward, 0.55f).normalized;
            w.Portal.transform.rotation = Quaternion.LookRotation(face) * Quaternion.Euler(0, 0, w.Age * 160f);
            w.Portal.transform.localScale = Vector3.one * size * (0.3f + 0.7f * open) * (1f + 0.25f * close);
            Tint(w.Portal, Hdr(new Color(light.r, light.g, light.b, portalA), 1.8f));
            w.Aura.transform.rotation = Cam.transform.rotation;
            Tint(w.Aura, new Color(tint.r, tint.g, tint.b, 0.35f * portalA));

            // Ship (or each craft): hidden until the portal is open, then flies in along its heading.
            int lanes = w.Streaks.Length;
            for (int i = 0; i < lanes; i++)
            {
                float laneDelay = small ? i * 0.07f : 0f;
                float p = (t - WarpOpen - laneDelay) / WarpFly;
                bool visible = p >= 0f;
                float dist = WarpDistance(maxDist, p);
                var offset = -w.Dir * dist;
                Transform part = small ? v.Crafts[i].T : v.Model;
                if (small)
                {
                    v.Crafts[i].R.enabled = visible && !v.Crafts[i].Lost;
                    part.position += offset;
                }
                else
                {
                    foreach (var r in v.Paint.renderers) r.enabled = visible;
                    v.Model.localPosition += offset;
                }

                // Light streak behind the arriving ship, longer the faster it is still going.
                var lr = w.Streaks[i];
                float speed = 3f * maxDist * Mathf.Pow(1f - Mathf.Clamp01(p), 2f) / WarpFly;
                bool streak = visible && p < 1.15f;
                lr.enabled = streak;
                if (streak)
                {
                    var head = part.position;
                    lr.SetPosition(0, head - w.Dir * Mathf.Min(2600f, speed * 0.12f + v.Length * 0.3f));
                    lr.SetPosition(1, head);
                    float fade = Mathf.Clamp01((1.15f - p) / 0.3f);
                    lr.startWidth = 0f;
                    lr.endWidth = (small ? 22f : v.Length * 0.22f) * fade;
                    lr.startColor = new Color(0, 0, 0, 0);
                    lr.endColor = Hdr(new Color(light.r, light.g, light.b, fade), 2f);
                }
            }

            // Arrival: a flash, a ring rushing outward, and sparks.
            if (!w.Flashed && t >= WarpOpen + WarpFly * 0.92f)
            {
                w.Flashed = true;
                var c = Centre(v);
                AddSprite("flash", glow, c, Hdr(light, 1.6f), size * 1.3f, 0.5f, Vector3.zero, false);
                var ringFx = AddSpriteGet("ring", warpRing, c, new Color(light.r, light.g, light.b, 0.8f), size * 1.4f, 0.6f, Vector3.zero, false);
                ringFx.Facing = Quaternion.LookRotation(Vector3.Slerp(w.Dir, -Cam.transform.forward, 0.55f).normalized);
                for (int k = 0; k < 10; k++)
                {
                    var d = (w.Dir + UnityEngine.Random.insideUnitSphere * 0.9f).normalized;
                    AddSprite("debris", warpSpark, c, Hdr(light, 1.4f), UnityEngine.Random.Range(30f, 70f), UnityEngine.Random.Range(0.4f, 0.8f), d * size * UnityEngine.Random.Range(0.4f, 1f), false);
                }
            }

            if (w.Age >= w.Total)
            {
                Kill(w.Portal.gameObject); Kill(w.Aura.gameObject);
                foreach (var lr in w.Streaks) Kill(lr.gameObject);
                v.Warp = null;
            }
        }

        public bool IsWarping(Ship s) => ViewOf(s)?.Warp != null;

        // ---------- dogfight areas ----------

        readonly Dictionary<int, SpriteRenderer> furballRings = new Dictionary<int, SpriteRenderer>();

        // A faint ring where each fight is, so it can be seen and clicked.
        void UpdateFurballRings()
        {
            var live = new HashSet<int>();
            foreach (var f in World.Furballs)
            {
                live.Add(f.Id);
                if (!furballRings.TryGetValue(f.Id, out var r))
                {
                    r = FlatSprite("fight area", ring, root, (float)f.Radius * 2f / 0.85f, 0f);
                    furballRings[f.Id] = r;
                }
                r.transform.position = ToWorld(f.Centre) + Vector3.up * 180f;
                float pulse = 0.09f + 0.04f * Mathf.Sin(Time.unscaledTime * 3f + f.Id);
                Tint(r, new Color(1f, 0.85f, 0.55f, pulse));
            }
            foreach (var id in furballRings.Keys.Where(k => !live.Contains(k)).ToList())
            {
                Kill(furballRings[id].gameObject);
                furballRings.Remove(id);
            }
        }

        // ---------- strike zones (small craft attacking a big ship) ----------

        public static float StrikeZoneRadius(ShipView big) => big.Length * 1.1f + 520f;

        readonly Dictionary<Ship, SpriteRenderer> strikeRings = new Dictionary<Ship, SpriteRenderer>();

        // Big ships under attack by small craft, with the craft attacking each.
        public Dictionary<Ship, List<Ship>> StrikeZones()
        {
            var zones = new Dictionary<Ship, List<Ship>>();
            foreach (var s in World.Alive)
            {
                var t = s.Target;
                if (!Hulls.IsSmall(s.Hull) || t == null || t.Dead || Hulls.IsSmall(t.Hull)) continue;
                var tv = ViewOf(t);
                if (tv == null || Vector3.Distance(ViewOf(s).Go.transform.position, tv.Go.transform.position) > 2400f) continue;
                if (!zones.TryGetValue(t, out var list)) zones[t] = list = new List<Ship>();
                list.Add(s);
            }
            return zones;
        }

        // The strike zone under a point on the play plane, if any (the big ship and its attackers).
        public (Ship big, List<Ship> attackers)? StrikeZoneAt(Vector3 ground)
        {
            foreach (var kv in StrikeZones())
            {
                var v = ViewOf(kv.Key);
                var p = v.Go.transform.position;
                if (new Vector2(ground.x - p.x, ground.z - p.z).magnitude <= StrikeZoneRadius(v)) return (kv.Key, kv.Value);
            }
            return null;
        }

        void UpdateStrikeRings()
        {
            var zones = StrikeZones();
            foreach (var kv in zones)
            {
                var v = ViewOf(kv.Key);
                if (!strikeRings.TryGetValue(kv.Key, out var r))
                {
                    r = FlatSprite("strike zone", ring, root, 1f, 0f);
                    strikeRings[kv.Key] = r;
                }
                r.transform.position = v.Go.transform.position + Vector3.up * 120f;
                r.transform.localScale = Vector3.one * StrikeZoneRadius(v) * 2f / 0.85f;
                float pulse = 0.08f + 0.035f * Mathf.Sin(Time.unscaledTime * 3.4f + kv.Key.Id);
                Tint(r, new Color(1f, 0.4f, 0.3f, pulse));
            }
            foreach (var ship in strikeRings.Keys.Where(k => !zones.ContainsKey(k)).ToList())
            {
                Kill(strikeRings[ship].gameObject);
                strikeRings.Remove(ship);
            }
        }

        // ---------- independent craft ----------

        public class Craft
        {
            public Transform T;
            public Renderer R;
            public Quaternion BaseRot;   // the craft's own rotation inside the squadron model
            public Vector3 Offset;       // formation slot, relative to the squadron, unrotated
            public Vector3 Pos, Vel, Facing;
            public float Phase, Dir, RadiusK, Bank;
            public bool Placed, Lost; // Lost: shot down (the squadron fights on with the rest)
            public LineRenderer Trail;
            public readonly Vector3[] Hist = new Vector3[TrailPoints];
            public int HistCount;
            public float SampleAge;
        }

        // Fading engine trail behind each craft (2017 fighters had trails). Drawn from recent positions so it
        // also works in the frame-by-frame recorder, where Unity's own trail timing doesn't run.
        const int TrailPoints = 30;
        const float TrailStep = 0.04f;

        void SetUpCrafts(ShipView v)
        {
            // The squadron model is built at heading 0, so each craft's pose relative to it is its formation slot.
            var anchor = v.Go.transform.position;
            v.Crafts = v.Paint.renderers.Select((r, i) =>
            {
                var c = new Craft
                {
                    T = r.transform, R = r, BaseRot = r.transform.rotation,
                    Offset = r.transform.position - anchor,
                    Phase = (float)(rng.NextDouble() * Math.PI * 2),
                    Dir = rng.NextDouble() < 0.5 ? -1f : 1f,
                    RadiusK = 0.8f + (float)rng.NextDouble() * 0.5f,
                };
                r.transform.SetParent(v.Go.transform, true); // free of the model's turning and banking
                var tr = new GameObject("trail").AddComponent<LineRenderer>();
                tr.transform.SetParent(v.Go.transform, false);
                tr.useWorldSpace = true; tr.positionCount = 0; tr.numCapVertices = 2;
                tr.sharedMaterial = addMat;
                tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                tr.widthCurve = AnimationCurve.Linear(0, 0, 1, 1);
                tr.widthMultiplier = 13f;
                c.Trail = tr;
                return c;
            }).ToArray();
        }

        void UpdateCrafts(ShipView v, float dt)
        {
            var s = v.Ship;
            var centre = v.Go.transform.position + Vector3.up * v.Height;
            var yaw = Quaternion.Euler(0, -(float)(s.Heading * Mathf.Rad2Deg), 0);
            var squadVel = yaw * Vector3.forward * (float)s.Speed;
            float maxSpeed = (float)s.Def.speed * 1.7f;

            var target = s.Target != null && !s.Target.Dead ? ViewOf(s.Target) : null;
            bool orbit = target != null && !Hulls.IsSmall(s.Target.Hull)
                         && Vector3.Distance(v.Go.transform.position, target.Go.transform.position) < 2400f;
            bool dogfight = target != null && Hulls.IsSmall(s.Target.Hull);

            bool straight = v.Pulse < v.StraightUntil;
            float cruise = maxSpeed * 0.8f;
            var straightYaw = straight ? Quaternion.LookRotation(v.StraightDir) : Quaternion.identity;
            foreach (var c in v.Crafts)
            {
                Vector3 goal, lead;
                if (straight)
                {
                    // Just out of the warp: in formation, straight along the jump line at cruise speed.
                    float flown = cruise * Mathf.Max(0f, v.Pulse - v.StraightFrom);
                    c.Pos = v.StraightAnchor + v.StraightDir * flown + straightYaw * c.Offset;
                    c.Vel = v.StraightDir * cruise;
                    c.Facing = v.StraightDir;
                    c.Bank = 0f; c.Placed = true;
                    c.T.position = c.Pos;
                    c.T.rotation = Quaternion.LookRotation(c.Facing) * c.BaseRot;
                    UpdateTrail(v, c, dt);
                    continue;
                }
                if (orbit)
                {
                    // Strike on a big ship: each craft has its own lane around it (radius across the strike zone,
                    // height, direction) and keeps making attack runs: dive in over the hull, pull back out.
                    int k = System.Array.IndexOf(v.Crafts, c);
                    float zone = StrikeZoneRadius(target);
                    float lane = Mathf.Lerp(target.Length * 0.55f + 140f, zone * 0.95f, Mathf.Repeat(c.Phase * 0.618f + k * 0.29f, 1f));
                    float th = c.Phase + c.Dir * (maxSpeed * 0.8f / lane) * v.Pulse;
                    float ahead = th + c.Dir * 0.4f;
                    var tc = Centre(target);
                    var lanePoint = tc + new Vector3(Mathf.Cos(ahead) * lane, 90f + Mathf.Sin(th * 1.6f + c.Phase) * 150f, Mathf.Sin(ahead) * lane);
                    var runPoint = tc + new Vector3(Mathf.Cos(th + Mathf.PI) * target.Length * 0.25f, 40f, Mathf.Sin(th + Mathf.PI) * target.Length * 0.25f);
                    float run = Mathf.Pow(Mathf.Max(0f, Mathf.Sin(v.Pulse * 0.45f + c.Phase * 1.7f)), 3f); // short dives
                    goal = Vector3.Lerp(lanePoint, runPoint, run * 0.75f);
                    lead = Vector3.zero;
                }
                else if (dogfight && s.Dogfighting && target.Crafts != null)
                {
                    // Furball: each craft gets on the tail of one enemy craft, inside a small shared area
                    // around the point between the two squadrons.
                    var centreOfFight = ToWorld(s.Furball.Centre) + Vector3.up * v.Height;
                    int k = System.Array.IndexOf(v.Crafts, c);
                    var live = target.Crafts.Where(x => x.R.enabled).ToArray();
                    var prey = live.Length > 0 ? live[(k + s.Id) % live.Length] : target.Crafts[0];
                    // Each craft has its own lane in the fight (a radius across the whole area and a height),
                    // and drifts between flying its lane and chasing an enemy's tail, so the fight fills the area
                    // instead of collapsing into one ball.
                    // Lanes lean toward the edge (square root spreads them evenly over the area, not the radius).
                    float R = (float)s.Furball.Radius;
                    float lane = Mathf.Lerp(0.45f, 1f, Mathf.Sqrt(Mathf.Repeat(c.Phase * 0.618f + k * 0.37f, 1f))) * R * 0.95f;
                    float th = c.Phase + c.Dir * (maxSpeed * 0.8f / lane) * v.Pulse;
                    var lanePoint = centreOfFight + new Vector3(
                        Mathf.Cos(th + 0.4f * c.Dir) * lane,
                        Mathf.Sin(th * 1.7f + c.Phase) * 170f,
                        Mathf.Sin(th + 0.4f * c.Dir) * lane);
                    var side = Vector3.Cross(prey.Facing, Vector3.up) * (c.Dir * 80f);
                    var tail = prey.Pos - prey.Facing * 140f + side;
                    // 0 = own lane, 1 = on a tail; chases are short bursts so most of the time the lanes hold.
                    float chase = Mathf.Pow(0.5f + 0.5f * Mathf.Sin(v.Pulse * 0.35f + c.Phase * 2f), 2f);
                    goal = Vector3.Lerp(lanePoint, tail, chase * 0.45f);
                    var fromCentre = goal - centreOfFight;
                    if (fromCentre.magnitude > R) goal = centreOfFight + fromCentre.normalized * R;
                    lead = Vector3.Lerp(Vector3.zero, prey.Vel * 0.5f, chase);
                }
                else if (dogfight)
                {
                    // Weaving around the squadron's position: crossing paths, climbing and diving.
                    const float R = 450f;
                    goal = centre + new Vector3(
                        Mathf.Sin(v.Pulse * 0.5f * c.RadiusK + c.Phase) * R,
                        Mathf.Sin(v.Pulse * 0.45f + c.Phase) * 110f,
                        Mathf.Cos(v.Pulse * 0.6f / c.RadiusK + c.Phase * 1.3f) * R);
                    lead = squadVel;
                }
                else
                {
                    goal = centre + yaw * c.Offset;
                    lead = squadVel;
                }

                if (!c.Placed || dt <= 0f)
                {
                    c.Pos = goal; c.Vel = lead; c.Placed = true;
                    c.Facing = yaw * Vector3.forward;
                }
                else
                {
                    var desired = (goal - c.Pos) * (orbit || dogfight ? 1.4f : 2.2f) + lead;
                    if (desired.magnitude > maxSpeed) desired = desired.normalized * maxSpeed;
                    c.Vel = Vector3.MoveTowards(c.Vel, desired, (orbit || dogfight ? 700f : 1400f) * dt);
                    c.Pos += c.Vel * dt;
                    if (c.Vel.sqrMagnitude > 400f)
                    {
                        var before = c.Facing;
                        c.Facing = Vector3.Slerp(c.Facing, c.Vel.normalized, Mathf.Clamp01(5f * dt));
                        float turn = Vector3.SignedAngle(before, c.Facing, Vector3.up) / Mathf.Max(dt, 1e-4f);
                        c.Bank = Mathf.Lerp(c.Bank, Mathf.Clamp(-turn * 0.5f, -65f, 65f), Mathf.Clamp01(6f * dt));
                    }
                }
                c.T.position = c.Pos;
                c.T.rotation = Quaternion.LookRotation(c.Facing) * Quaternion.Euler(0, 0, c.Bank) * c.BaseRot;
                UpdateTrail(v, c, dt);
            }
        }

        void UpdateTrail(ShipView v, Craft c, float dt)
        {
            // Contrails start the moment the craft is out of the jump (it flies straight on from there).
            bool show = c.R.enabled && (v.Warp == null || v.Pulse >= v.StraightFrom);
            if (!show) { c.HistCount = 0; c.Trail.positionCount = 0; return; }
            // A trail point every TrailStep seconds (not every frame), so the contrail is the same length
            // (TrailPoints x TrailStep = 1.2 s of flight) at any frame rate. The newest end follows the craft live.
            var tail = c.Pos - c.Facing * 12f;
            if (c.HistCount == 0) { c.Hist[0] = tail; c.HistCount = 1; c.SampleAge = 0f; }
            c.SampleAge += dt;
            if (c.SampleAge >= TrailStep)
            {
                c.SampleAge = 0f;
                for (int k = Mathf.Min(c.HistCount, TrailPoints - 1); k > 0; k--) c.Hist[k] = c.Hist[k - 1];
                c.HistCount = Mathf.Min(c.HistCount + 1, TrailPoints);
            }
            c.Hist[0] = tail;
            int n = c.HistCount;
            c.Trail.positionCount = n;
            for (int k = 0; k < n; k++) c.Trail.SetPosition(k, c.Hist[n - 1 - k]); // oldest first
            var tint = Color.Lerp(TeamTint[v.Ship.Team % TeamTint.Length], Color.white, 0.55f);
            c.Trail.startColor = new Color(0, 0, 0, 0);
            c.Trail.endColor = Hdr(new Color(tint.r, tint.g, tint.b, 0.5f), 1.2f);
        }

        // A live craft of a squadron, at random (for muzzles and aim points).
        Craft AnyCraft(ShipView v)
        {
            var live = v.Crafts.Where(c => c.R.enabled).ToArray();
            return live.Length == 0 ? v.Crafts[0] : live[rng.Next(live.Length)];
        }
    }
}
