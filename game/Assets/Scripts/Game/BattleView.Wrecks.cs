using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Saga.Game
{
    // Deaths that play out for a few seconds instead of a ship just vanishing:
    //   Big ships break up: the hull splits into chunks along jagged cracks (Saga/Stack _Clip), the chunks drift apart
    //   and tumble slowly, burn at the breaks with secondary blasts, char, and sink away.
    //   Craft (fighters, bombers, scouts) shot down keep flying a moment, rolling and trailing fire, then blow up.
    // Purely visual: the rules have already removed the ship. Wrecks copy the visible meshes (pixel stacks, or the 3D
    // model when stacks are off; 3D models break into one piece only, as they can't be cut).
    public partial class BattleView
    {
        class Wreck
        {
            public Transform T;
            public Vector3 Vel, Spin;          // units/s; degrees/s about world x, y, z
            public float Age, Life, Size, NextEmber, BlastsLeft, NextBlast;
            public List<Renderer> Parts = new List<Renderer>();
            public Bounds Local;               // chunk extent in its own space, for fire and blast points
            public float BreakA, BreakB;       // local z of the cracks (NaN: an outer end)
        }

        class Faller
        {
            public Transform T;
            public Vector3 Vel, Axis;
            public float Age, BlowAt, Roll, NextSmoke, Size;
        }

        readonly List<Wreck> wrecks = new List<Wreck>();
        readonly List<Faller> fallers = new List<Faller>();
        readonly MaterialPropertyBlock wreckMpb = new MaterialPropertyBlock();
        static readonly int ClipId = Shader.PropertyToID("_Clip"), ColorId = Shader.PropertyToID("_Color"), BaseColorId = Shader.PropertyToID("_BaseColor");

        // ---------- big ships ----------

        void BreakUp(ShipView v)
        {
            var stacks = VisibleMeshes(v.Go.transform).ToList();
            if (stacks.Count == 0) return;
            var main = stacks.FirstOrDefault(r => r.name == "stack") as MeshRenderer;
            int pieces = main == null ? 1 : v.Ship.Hull == Saga.Sim.Hull.Frigate ? 2 : v.Ship.Hull == Saga.Sim.Hull.Destroyer ? 3 : 4;
            var centre = Centre(v);
            var shipVel = Quaternion.Euler(0, -(float)(v.Ship.Heading * Mathf.Rad2Deg), 0) * Vector3.forward * Mathf.Min(400f, (float)v.Ship.Speed);

            // Cracks at uneven places along the hull.
            float zMin = -1e5f, zMax = 1e5f, pixel = 1f;
            var cuts = new List<float>();
            if (main != null)
            {
                var mb = main.GetComponent<MeshFilter>().sharedMesh.bounds;
                zMin = mb.min.z; zMax = mb.max.z;
                pixel = SpriteStacks.Pixel(main.GetComponent<MeshFilter>().sharedMesh);
                for (int i = 1; i < pieces; i++)
                    cuts.Add(Mathf.Lerp(zMin, zMax, (i + Random.Range(-0.3f, 0.3f)) / pieces));
                cuts.Sort();
            }
            for (int p = 0; p < pieces; p++)
            {
                float a = p == 0 ? float.NaN : cuts[p - 1], b = p == pieces - 1 ? float.NaN : cuts[p];
                var w = new Wreck { Life = Random.Range(8f, 11f), Size = v.Length, BreakA = a, BreakB = b };
                // Pivot in the middle of the chunk so it tumbles about itself.
                Vector3 mid = centre;
                if (main != null)
                {
                    var mb = main.GetComponent<MeshFilter>().sharedMesh.bounds;
                    float z0 = float.IsNaN(a) ? zMin : a, z1 = float.IsNaN(b) ? zMax : b;
                    w.Local = new Bounds(new Vector3(mb.center.x, mb.center.y, (z0 + z1) * 0.5f), new Vector3(mb.size.x, mb.size.y, z1 - z0));
                    mid = main.transform.TransformPoint(w.Local.center);
                }
                var go = new GameObject("wreck");
                go.transform.SetParent(root, false);
                go.transform.position = mid;
                w.T = go.transform;
                foreach (var r in stacks)
                {
                    bool isStack = r.name == "stack";
                    if (main != null && !isStack) continue;
                    var copy = Copy(r, w.T);
                    w.Parts.Add(copy);
                    if (isStack && pieces > 1)
                    {
                        float lo = float.IsNaN(a) ? -1e5f : a, hi = float.IsNaN(b) ? 1e5f : b;
                        copy.GetPropertyBlock(wreckMpb);
                        wreckMpb.SetVector(ClipId, new Vector4(lo, hi, pixel * 3f, pixel));
                        copy.SetPropertyBlock(wreckMpb);
                    }
                }
                // Apart from each other along the hull, carrying some of the ship's way, slowly turning.
                var away = mid - centre; away.y = 0;
                var dir = away.sqrMagnitude > 1f ? away.normalized : Random.insideUnitSphere.normalized;
                w.Vel = shipVel * 0.5f + dir * Random.Range(0.06f, 0.14f) * v.Length + Vector3.down * Random.Range(4f, 12f);
                w.Spin = new Vector3(Random.Range(-6f, 6f), Random.Range(-14f, 14f), Random.Range(-8f, 8f));
                w.BlastsLeft = Random.Range(2, 5);
                w.NextBlast = Random.Range(0.2f, 0.9f);
                wrecks.Add(w);
            }
        }

        void UpdateWrecks(float dt)
        {
            for (int i = wrecks.Count - 1; i >= 0; i--)
            {
                var w = wrecks[i];
                if (w.T == null) { wrecks.RemoveAt(i); continue; }
                w.Age += dt;
                if (w.Age > w.Life) { Kill(w.T.gameObject); wrecks.RemoveAt(i); continue; }
                w.Vel *= 1f - 0.25f * dt;
                w.T.position += w.Vel * dt;
                w.T.rotation = Quaternion.Euler(w.Spin * dt) * w.T.rotation;
                // Chars over the first seconds, then fades into the dark as it sinks away.
                float k = w.Age / w.Life;
                float gone = Mathf.Clamp01((k - 0.65f) / 0.35f);
                gone = gone * gone * (3f - 2f * gone); // eased 0 -> 1 over the last third
                float shade = Mathf.Lerp(1f, 0.45f, Mathf.Clamp01(w.Age / 6f)) * (1f - gone);
                var tint = new Color(shade, shade * 0.92f, shade * 0.86f, 1f);
                foreach (var r in w.Parts)
                {
                    r.GetPropertyBlock(wreckMpb);
                    wreckMpb.SetColor(ColorId, tint); wreckMpb.SetColor(BaseColorId, tint);
                    r.SetPropertyBlock(wreckMpb);
                }
                if (k > 0.6f) w.Vel += Vector3.down * 30f * dt;

                // Fire along the breaks, dying down.
                float burn = 1f - Mathf.Clamp01(w.Age / 5.5f);
                if (burn > 0f && w.Age >= w.NextEmber)
                {
                    w.NextEmber = w.Age + Mathf.Lerp(0.18f, 0.05f, burn);
                    var at = FirePoint(w);
                    AddSprite("flash", glow, at, Hdr(new Color(1f, 0.5f, 0.15f), 1.6f * burn + 0.4f), w.Size * Random.Range(0.06f, 0.12f), Random.Range(0.3f, 0.6f), w.Vel + Random.insideUnitSphere * 20f, false);
                    if (Random.value < 0.4f)
                        AddSprite("debris", dot, at, Hdr(new Color(1f, 0.65f, 0.3f), 1.6f), Random.Range(4f, 9f), Random.Range(0.5f, 1f), w.Vel + Random.insideUnitSphere * 90f, false);
                }
                // A few secondary blasts in the first seconds.
                if (w.BlastsLeft > 0 && w.Age >= w.NextBlast)
                {
                    w.BlastsLeft--;
                    w.NextBlast = w.Age + Random.Range(0.4f, 1.3f);
                    var at = w.T.TransformPoint(RandomIn(w.Local));
                    float s = w.Size * Random.Range(0.25f, 0.45f);
                    AddSprite("flash", glow, at, Hdr(new Color(1f, 0.9f, 0.7f), 2.6f), s * 0.4f, 0.12f, w.Vel, false);
                    AddSprite("flash", glow, at, Hdr(new Color(1f, 0.55f, 0.2f), 1.9f), s, 0.6f, w.Vel, false);
                    for (int d = 0; d < 6; d++)
                        AddSprite("debris", dot, at, Hdr(new Color(1f, 0.6f, 0.25f), 1.8f), Random.Range(6f, 12f), Random.Range(0.5f, 1f), w.Vel + Random.onUnitSphere * Random.Range(80f, 200f), false);
                    SoundBoard.Current?.Explosion(at, false);
                }
            }
        }

        Vector3 FirePoint(Wreck w)
        {
            var b = w.Local;
            var local = RandomIn(b);
            // Mostly at the cracks, where the hull tore.
            bool hasA = !float.IsNaN(w.BreakA), hasB = !float.IsNaN(w.BreakB);
            if ((hasA || hasB) && Random.value < 0.8f)
                local.z = hasA && (!hasB || Random.value < 0.5f) ? w.BreakA : w.BreakB;
            return w.T.TransformPoint(local);
        }

        static Vector3 RandomIn(Bounds b) => b.center + Vector3.Scale(b.extents, new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(-0.5f, 0.5f), Random.Range(-0.8f, 0.8f)));

        // ---------- craft ----------

        // A craft just shot down: it carries on a short way, rolling, nose dropping, trailing fire, then blows up.
        void ShootDown(Craft c, float delay)
        {
            var parts = VisibleMeshes(c.T).ToList();
            if (parts.Count == 0) { CraftBlast(c.T.position, c.Vel); return; }
            var go = new GameObject("falling craft");
            go.transform.SetParent(root, false);
            go.transform.SetPositionAndRotation(c.T.position, c.T.rotation);
            foreach (var r in parts) Copy(r, go.transform);
            var vel = c.Vel;
            var fwd = c.T.forward; fwd.y = 0;
            if (vel.sqrMagnitude < 60f * 60f) vel = (fwd.sqrMagnitude > 0.01f ? fwd.normalized : Vector3.forward) * 260f;
            fallers.Add(new Faller
            {
                T = go.transform, Vel = vel, Axis = vel.normalized, Size = c.R != null ? c.R.bounds.size.magnitude : 40f,
                BlowAt = delay + Random.Range(0.7f, 1.4f), Roll = Random.Range(360f, 720f) * (Random.value < 0.5f ? -1f : 1f),
            });
            // A hit flash where it was struck.
            AddSprite("flash", glow, c.T.position, Hdr(new Color(1f, 0.85f, 0.6f), 2.4f), 45f, 0.12f, Vector3.zero, false);
        }

        void UpdateFallers(float dt)
        {
            for (int i = fallers.Count - 1; i >= 0; i--)
            {
                var f = fallers[i];
                if (f.T == null) { fallers.RemoveAt(i); continue; }
                f.Age += dt;
                if (f.Age >= f.BlowAt)
                {
                    CraftBlast(f.T.position, f.Vel);
                    Kill(f.T.gameObject);
                    fallers.RemoveAt(i);
                    continue;
                }
                // Losing way and dropping, the nose following the path down, rolling about it.
                f.Vel *= 1f - 0.35f * dt;
                f.Vel += Vector3.down * 90f * dt;
                f.T.position += f.Vel * dt;
                var look = f.Vel.sqrMagnitude > 1f ? Quaternion.LookRotation(f.Vel.normalized) : f.T.rotation;
                f.T.rotation = look * Quaternion.Euler(0, 0, f.Roll * f.Age);
                if (f.Age >= f.NextSmoke)
                {
                    f.NextSmoke = f.Age + 0.035f;
                    AddSprite("flash", glow, f.T.position, Hdr(new Color(1f, 0.55f, 0.2f), 1.5f), Random.Range(14f, 24f), Random.Range(0.25f, 0.45f), Random.insideUnitSphere * 12f, false);
                }
            }
        }

        void CraftBlast(Vector3 p, Vector3 vel)
        {
            AddSprite("flash", glow, p, Hdr(new Color(1f, 0.95f, 0.8f), 3f), 50f, 0.1f, Vector3.zero, false);
            AddSprite("flash", glow, p, Hdr(new Color(1f, 0.6f, 0.25f), 2.2f), 110f, 0.55f, vel * 0.3f, false);
            AddSprite("ring", warpRing, p, Hdr(new Color(1f, 0.7f, 0.4f), 1.3f), 160f, 0.3f, Vector3.zero, false);
            for (int i = 0; i < 10; i++)
            {
                var d = (vel.normalized * 0.6f + Random.insideUnitSphere).normalized;
                AddSprite("debris", dot, p, Hdr(new Color(1f, 0.6f, 0.25f), 1.8f), Random.Range(5f, 11f), Random.Range(0.5f, 1f), d * Random.Range(80f, 220f), false);
            }
            SoundBoard.Current?.Explosion(p, false);
        }

        // ---------- shared ----------

        // What the player can see of a ship or craft right now: its pixel stacks, or its 3D parts.
        static IEnumerable<Renderer> VisibleMeshes(Transform t) =>
            t.GetComponentsInChildren<MeshRenderer>().Where(r => r.enabled && !r.forceRenderingOff && r.GetComponent<MeshFilter>() != null);

        // A still copy of a mesh renderer, keeping its place in the world, under a new parent.
        static Renderer Copy(Renderer r, Transform parent)
        {
            var go = new GameObject(r.name);
            go.transform.SetPositionAndRotation(r.transform.position, r.transform.rotation);
            go.transform.localScale = r.transform.lossyScale;
            go.transform.SetParent(parent, true);
            go.AddComponent<MeshFilter>().sharedMesh = r.GetComponent<MeshFilter>().sharedMesh;
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterials = r.sharedMaterials;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return mr;
        }
    }
}
