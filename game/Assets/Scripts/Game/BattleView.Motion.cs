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
            w.Portal.transform.SetParent(v.Go.transform, false);
            w.Portal.transform.localPosition = Vector3.up * v.Height;
            w.Portal.transform.localScale = Vector3.one * 0.01f;
            w.Aura = new GameObject("warp aura").AddComponent<SpriteRenderer>();
            w.Aura.sprite = warpAura; w.Aura.sharedMaterial = addMat;
            w.Aura.transform.SetParent(v.Go.transform, false);
            w.Aura.transform.localPosition = Vector3.up * v.Height;
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
                    v.Crafts[i].R.enabled = visible && i < World.CraftLeft(v.Ship);
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

        // ---------- independent craft ----------

        public class Craft
        {
            public Transform T;
            public Renderer R;
            public Quaternion BaseRot;   // the craft's own rotation inside the squadron model
            public Vector3 Offset;       // formation slot, relative to the squadron, unrotated
            public Vector3 Pos, Vel, Facing;
            public float Phase, Dir, RadiusK, Bank;
            public bool Placed;
            public LineRenderer Trail;
            public readonly Vector3[] Hist = new Vector3[TrailPoints];
            public int HistCount;
        }

        // Fading engine trail behind each craft (2017 fighters had trails). Drawn from recent positions so it
        // also works in the frame-by-frame recorder, where Unity's own trail timing doesn't run.
        const int TrailPoints = 28;

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
                tr.widthMultiplier = 18f;
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

            foreach (var c in v.Crafts)
            {
                Vector3 goal, lead;
                if (orbit)
                {
                    // Each craft circles the big ship at its own radius, height and direction, aiming a little
                    // ahead on the circle so it chases smoothly.
                    float r = (target.Length * 0.75f + 280f) * c.RadiusK;
                    float th = c.Phase + c.Dir * (maxSpeed * 0.75f / r) * v.Pulse;
                    float ahead = th + c.Dir * 0.4f;
                    goal = Centre(target) + new Vector3(Mathf.Cos(ahead) * r, 70f + Mathf.Sin(th * 2f + c.Phase) * 90f, Mathf.Sin(ahead) * r);
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
                    var side = Vector3.Cross(prey.Facing, Vector3.up) * (c.Dir * 60f);
                    goal = prey.Pos - prey.Facing * 110f + side;
                    var fromCentre = goal - centreOfFight;
                    if (fromCentre.magnitude > 480f) goal = centreOfFight + fromCentre.normalized * 480f;
                    lead = prey.Vel * 0.6f;
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
            bool show = c.R.enabled && v.Warp == null;
            if (!show) { c.HistCount = 0; c.Trail.positionCount = 0; return; }
            if (dt > 0f)
            {
                for (int k = Mathf.Min(c.HistCount, TrailPoints - 1); k > 0; k--) c.Hist[k] = c.Hist[k - 1];
                c.Hist[0] = c.Pos - c.Facing * 12f;
                c.HistCount = Mathf.Min(c.HistCount + 1, TrailPoints);
            }
            int n = c.HistCount;
            c.Trail.positionCount = n;
            for (int k = 0; k < n; k++) c.Trail.SetPosition(k, c.Hist[n - 1 - k]); // oldest first
            var tint = Color.Lerp(TeamTint[v.Ship.Team % TeamTint.Length], Color.white, 0.55f);
            c.Trail.startColor = new Color(0, 0, 0, 0);
            c.Trail.endColor = Hdr(new Color(tint.r, tint.g, tint.b, 0.7f), 1.4f);
        }

        // A live craft of a squadron, at random (for muzzles and aim points).
        Craft AnyCraft(ShipView v)
        {
            var live = v.Crafts.Where(c => c.R.enabled).ToArray();
            return live.Length == 0 ? v.Crafts[0] : live[rng.Next(live.Length)];
        }
    }
}
