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
            public Bounds Local;               // chunk extent in the stack's model space, for fire and blast points
            public Transform Space;            // the stack copy, whose model space Local is in (null: 3D, one piece)
            public bool HasA, HasB;            // a crack below / above (false: an outer end of the hull)
            public Vector2 CrackA, CrackB;     // crack directions (x, z), as in the shader
            public float OffA, OffB, Small;    // crack offsets; Small: the chunk's share of the hull length
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
        static readonly int ClipAId = Shader.PropertyToID("_ClipA"), ClipBId = Shader.PropertyToID("_ClipB"), ColorId = Shader.PropertyToID("_Color"), BaseColorId = Shader.PropertyToID("_BaseColor");

        // ---------- big ships ----------

        // A crack across the hull: points p with dot(p.xz, Dir) = Offset in model space. Dir leans up to about
        // 35 degrees off straight across, so breaks run at angles.
        struct Crack { public Vector2 Dir; public float Offset; }

        void BreakUp(ShipView v)
        {
            var stacks = VisibleMeshes(v.Go.transform).ToList();
            if (stacks.Count == 0) return;
            var main = stacks.FirstOrDefault(r => r.name == "stack") as MeshRenderer;
            // Bigger ships tend to break into more pieces, but not always the same number.
            var hull = v.Ship.Hull;
            int pieces = main == null ? 1
                : hull == Saga.Sim.Hull.Frigate ? Random.Range(1, 3)
                : hull == Saga.Sim.Hull.Destroyer ? Random.Range(2, 4)
                : hull == Saga.Sim.Hull.SuperCapital ? Random.Range(3, 6) : Random.Range(2, 5);
            var centre = Centre(v);
            var shipVel = Quaternion.Euler(0, -(float)(v.Ship.Heading * Mathf.Rad2Deg), 0) * Vector3.forward * Mathf.Min(400f, (float)v.Ship.Speed);

            Bounds mb = default;
            float pixel = 1f;
            var cracks = new List<Crack>();
            if (main != null)
            {
                var mesh = main.GetComponent<MeshFilter>().sharedMesh;
                mb = mesh.bounds;
                pixel = SpriteStacks.Pixel(mesh);
                // Cracks anywhere along the hull (each piece at least an eighth of it), not evenly spaced: sometimes
                // the bow or the engines just snap off, sometimes it goes near the middle.
                var at = new List<float>();
                for (int tries = 0; at.Count < pieces - 1 && tries < 40; tries++)
                {
                    float f = Random.Range(0.12f, 0.88f);
                    if (at.All(x => Mathf.Abs(x - f) > 0.14f)) at.Add(f);
                }
                at.Sort();
                foreach (var f in at)
                {
                    float angle = Random.Range(-35f, 35f) * Mathf.Deg2Rad;
                    var dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)); // (x, z), mostly along the hull
                    var through = new Vector2(mb.center.x + Random.Range(-0.2f, 0.2f) * mb.size.x, Mathf.Lerp(mb.min.z, mb.max.z, f));
                    cracks.Add(new Crack { Dir = dir, Offset = Vector2.Dot(through, dir) });
                }
                pieces = cracks.Count + 1;
            }
            for (int p = 0; p < pieces; p++)
            {
                bool hasA = p > 0, hasB = p < pieces - 1;
                var a = hasA ? cracks[p - 1] : new Crack { Dir = new Vector2(0, 1), Offset = -1e5f };
                var b = hasB ? cracks[p] : new Crack { Dir = new Vector2(0, 1), Offset = 1e5f };
                var w = new Wreck { Life = Random.Range(8f, 11f), Size = v.Length, HasA = hasA, HasB = hasB, CrackA = a.Dir, CrackB = b.Dir, OffA = a.Offset, OffB = b.Offset, Small = 1f };
                Vector3 mid = centre;
                if (main != null)
                {
                    // The chunk's length: where its cracks cross the hull's centre line.
                    float z0 = hasA ? (a.Offset - a.Dir.x * mb.center.x) / a.Dir.y : mb.min.z;
                    float z1 = hasB ? (b.Offset - b.Dir.x * mb.center.x) / b.Dir.y : mb.max.z;
                    z0 = Mathf.Max(z0, mb.min.z); z1 = Mathf.Min(z1, mb.max.z);
                    if (z1 < z0) (z0, z1) = (z1, z0);
                    w.Local = new Bounds(new Vector3(mb.center.x, mb.center.y, (z0 + z1) * 0.5f), new Vector3(mb.size.x, mb.size.y, Mathf.Max(1f, z1 - z0)));
                    mid = main.transform.TransformPoint(w.Local.center);
                    w.Small = (z1 - z0) / Mathf.Max(1f, mb.size.z);
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
                        copy.GetPropertyBlock(wreckMpb);
                        wreckMpb.SetVector(ClipAId, new Vector4(a.Offset, a.Dir.x, a.Dir.y, pixel * 3f));
                        wreckMpb.SetVector(ClipBId, new Vector4(b.Offset, b.Dir.x, b.Dir.y, pixel));
                        copy.SetPropertyBlock(wreckMpb);
                    }
                }
                w.Space = main != null ? w.Parts[0].transform : null;
                // Apart from each other along the hull, carrying some of the ship's way, slowly turning. Small pieces
                // are thrown harder and spin faster than big ones.
                var away = mid - centre; away.y = 0;
                var outDir = away.sqrMagnitude > 1f ? away.normalized : Random.insideUnitSphere.normalized;
                float light = Mathf.Lerp(1.8f, 0.7f, Mathf.Clamp01(w.Small * 1.5f));
                w.Vel = shipVel * Random.Range(0.3f, 0.7f) + outDir * Random.Range(0.04f, 0.14f) * v.Length * light
                        + Random.insideUnitSphere * 0.03f * v.Length + Vector3.down * Random.Range(4f, 12f);
                w.Spin = new Vector3(Random.Range(-6f, 6f), Random.Range(-14f, 14f), Random.Range(-8f, 8f)) * light;
                w.BlastsLeft = Random.Range(1, 5);
                w.NextBlast = Random.Range(0.2f, 1.2f);
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
                    var at = Where(w, RandomIn(w.Local));
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
            var local = RandomIn(w.Local);
            // Mostly on the cracks, where the hull tore.
            if ((w.HasA || w.HasB) && Random.value < 0.8f)
            {
                bool useA = w.HasA && (!w.HasB || Random.value < 0.5f);
                var dir = useA ? w.CrackA : w.CrackB;
                float off = useA ? w.OffA : w.OffB;
                local.z = (off - dir.x * local.x) / dir.y;
            }
            return Where(w, local);
        }

        // A point in the chunk's model space, in the world (the chunk has moved and turned since).
        static Vector3 Where(Wreck w, Vector3 local) => w.Space != null ? w.Space.TransformPoint(local) : w.T.position;

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
