using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Saga.Game
{
    // Sprite-stacked ships and scenery: each model is drawn as a pile of flat pixel-art slices at their real heights,
    // so the tilted battle camera sees a chunky, voxel-like ship (look test: docs/sprite-stack.html).
    //
    // Data (tools/sprite_stack.py): per model an 8-bit atlas of the slices (Art/Stacks/<name>.png, palette index per
    // pixel, bit 7 = top surface) and placement plus palettes in stacks.json / scenery_stacks.json. One mesh of stacked
    // quads per model; the Saga/Stack shader looks each pixel up in the team's palette and shades lower slices darker.
    // Meshes and materials are cached, so every ship of a kind and colour shares them.
    //
    // The 3D model stays in place, hidden, so turrets, muzzles, hit colliders and engine glows keep working; a
    // StackFollow on each stack copies the model's visibility (warp-in, lost craft) every frame. The setting is
    // remembered and can be switched in the pause menu at any time.
    public static class SpriteStacks
    {
        const string Key = "saga.ships.stacked";
        public static bool On
        {
            get => PlayerPrefs.GetInt(Key, 1) == 1;
            set { PlayerPrefs.SetInt(Key, value ? 1 : 0); PlayerPrefs.Save(); }
        }

        [Serializable]
        public class Info
        {
            public string sprite;              // atlas name in Art/Stacks
            public int part = -1;              // scenery: renderer index in the map prefab
            public int slices, w, h, cols, rows;
            public float unit, left, top, miny, sizey; // model space: pixel size, north-west corner, height range
            public int[] blue, red, pal;       // palettes, RGB triples (ships: blue and red; scenery: pal)
        }
        [Serializable] class ShipList { public List<Info> ships = new List<Info>(); }
        [Serializable] class SceneryMap { public string id; public List<Info> parts = new List<Info>(); }
        [Serializable] class SceneryList { public List<SceneryMap> maps = new List<SceneryMap>(); }

        static ShipLibrary loaded;
        static Dictionary<string, Info> ships;
        static Dictionary<string, List<Info>> maps;
        static readonly Dictionary<string, (Mesh mesh, Material mat)> cache = new Dictionary<string, (Mesh, Material)>();

        static void Load(ShipLibrary lib)
        {
            if (loaded == lib) return;
            loaded = lib;
            ships = new Dictionary<string, Info>();
            maps = new Dictionary<string, List<Info>>();
            if (lib.stacksJson != null)
                foreach (var i in JsonUtility.FromJson<ShipList>(lib.stacksJson.text).ships) ships[i.sprite] = i;
            if (lib.sceneryStacksJson != null)
                foreach (var m in JsonUtility.FromJson<SceneryList>(lib.sceneryStacksJson.text).maps) maps[m.id] = m.parts;
        }

        public static bool HasShip(ShipLibrary lib, string sprite)
        {
            Load(lib);
            return lib.stackShader != null && sprite != null && ships.ContainsKey(sprite.Replace('/', '_'));
        }

        // Gives a ship model its stack. Squadrons get one per craft, riding on that craft.
        public static void AttachShip(ShipLibrary lib, ShipModel model, string sprite, string colour, bool squadron)
        {
            if (!HasShip(lib, sprite)) return;
            var info = ships[sprite.Replace('/', '_')];
            var (mesh, mat) = Get(lib, info, colour == "Red" ? info.red : info.blue, info.sprite + "_" + colour);
            if (mesh == null) return;
            var parts = model.renderers;
            if (!squadron)
            {
                var go = Make(model.transform, mesh, mat);
                StackFollow.Add(go, parts);
                return;
            }
            // The slices were cut from one craft where it sits in the squadron model. Find that craft, read the stack's
            // pose relative to it, and give every craft the same.
            var centre = model.transform.TransformPoint(new Vector3(info.left + info.w * info.unit * 0.5f, 0f, info.top - info.h * info.unit * 0.5f));
            var first = parts.OrderBy(r => Flat(r.bounds.center - centre)).First();
            var probe = Make(model.transform, mesh, mat);
            probe.transform.SetParent(first.transform, true);
            Vector3 lp = probe.transform.localPosition, ls = probe.transform.localScale;
            Quaternion lr = probe.transform.localRotation;
            if (Application.isPlaying) UnityEngine.Object.Destroy(probe); else UnityEngine.Object.DestroyImmediate(probe);
            foreach (var r in parts)
            {
                var go = Make(r.transform, mesh, mat);
                go.transform.localPosition = lp; go.transform.localRotation = lr; go.transform.localScale = ls;
                StackFollow.Add(go, new Renderer[] { r });
            }
        }

        // Swaps a battle map's scenery for stacks (cut in the map prefab's own space by StackScenery).
        public static void AttachScenery(ShipLibrary lib, GameObject map, string id)
        {
            Load(lib);
            if (lib.stackShader == null || id == null || !maps.TryGetValue(id, out var parts)) return;
            var renderers = map.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var info in parts)
            {
                if (info.part < 0 || info.part >= renderers.Length) continue;
                var (mesh, mat) = Get(lib, info, info.pal, info.sprite);
                if (mesh == null) continue;
                var go = Make(map.transform, mesh, mat);
                StackFollow.Add(go, new Renderer[] { renderers[info.part] });
            }
        }

        static float Flat(Vector3 d) => d.x * d.x + d.z * d.z;

        static GameObject Make(Transform parent, Mesh mesh, Material mat)
        {
            var go = new GameObject("stack");
            go.transform.SetParent(parent, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
            return go;
        }

        static readonly Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();

        // The mesh is shared by every paint of a model; the material carries the paint's palette.
        static (Mesh, Material) Get(ShipLibrary lib, Info info, int[] palette, string key)
        {
            if (cache.TryGetValue(key, out var have) && have.mesh != null) return have;
            var atlas = lib.StackAtlas(info.sprite);
            if (atlas == null || palette == null || palette.Length < 3) return (null, null);
            if (!meshes.TryGetValue(info.sprite, out var mesh) || mesh == null)
                meshes[info.sprite] = mesh = BuildMesh(info, atlas.width, atlas.height);
            var mat = new Material(lib.stackShader) { mainTexture = atlas, name = "stack " + key };
            mat.SetTexture("_Palette", Palette(palette, key));
            var made = (mesh, mat);
            cache[key] = made;
            return made;
        }

        // Palette colours (sRGB, as baked) in a one-row texture the shader reads by index.
        static Texture2D Palette(int[] pal, string key)
        {
            int n = pal.Length / 3;
            var t = new Texture2D(n, 1, TextureFormat.RGBA32, false, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp, name = "palette " + key };
            var px = new Color32[n];
            for (int i = 0; i < n; i++) px[i] = new Color32((byte)pal[i * 3], (byte)pal[i * 3 + 1], (byte)pal[i * 3 + 2], 255);
            t.SetPixels32(px);
            t.Apply(false, true);
            return t;
        }

        // One horizontal quad per slice, bottom to top, each showing its cell of the atlas.
        static Mesh BuildMesh(Info info, int W, int H)
        {
            int n = info.slices;
            var v = new Vector3[n * 4];
            var uv = new Vector2[n * 4];
            var tris = new int[n * 6];
            var col = new Color32[n * 4];
            float x0 = info.left, x1 = info.left + info.w * info.unit, z1 = info.top, z0 = info.top - info.h * info.unit;
            for (int k = 0; k < n; k++)
            {
                float y = info.miny + (k + 0.5f) * info.sizey / n;
                int r = k / info.cols, c = k % info.cols;
                float u0 = (float)(c * info.w) / W, u1 = (float)((c + 1) * info.w) / W;
                float vt = 1f - (float)(r * info.h) / H, vb = 1f - (float)((r + 1) * info.h) / H;
                int b = k * 4;
                v[b] = new Vector3(x0, y, z0); uv[b] = new Vector2(u0, vb);
                v[b + 1] = new Vector3(x0, y, z1); uv[b + 1] = new Vector2(u0, vt);
                v[b + 2] = new Vector3(x1, y, z1); uv[b + 2] = new Vector2(u1, vt);
                v[b + 3] = new Vector3(x1, y, z0); uv[b + 3] = new Vector2(u1, vb);
                // Lower slices darker; the shader keeps the top surface bright and rounds to a few steps.
                byte shade = (byte)(255f * (0.55f + 0.45f * k / Mathf.Max(1, n - 1)));
                col[b] = col[b + 1] = col[b + 2] = col[b + 3] = new Color32(shade, shade, shade, 255);
                int t = k * 6;
                tris[t] = b; tris[t + 1] = b + 1; tris[t + 2] = b + 2; tris[t + 3] = b; tris[t + 4] = b + 2; tris[t + 5] = b + 3;
            }
            var m = new Mesh { name = "stack " + info.sprite };
            if (v.Length > 65000) m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            m.vertices = v; m.uv = uv; m.colors32 = col; m.triangles = tris;
            m.RecalculateBounds();
            return m;
        }
    }

    // Keeps a stack showing exactly when its 3D model would, and hides the model while stacks are on.
    public class StackFollow : MonoBehaviour
    {
        public Renderer[] Sources;
        MeshRenderer mine;
        static readonly List<StackFollow> live = new List<StackFollow>();

        public static StackFollow Add(GameObject stack, Renderer[] sources)
        {
            var f = stack.AddComponent<StackFollow>();
            f.Sources = sources;
            live.RemoveAll(x => x == null);
            live.Add(f);
            f.LateUpdate();
            return f;
        }

        // For editor tools that render without the player loop (BattlePreview).
        public static void SyncAll() { foreach (var f in live) if (f != null) f.LateUpdate(); }

        void LateUpdate()
        {
            if (Sources == null || Sources.Length == 0 || Sources[0] == null) return;
            bool on = SpriteStacks.On;
            foreach (var s in Sources) if (s != null) s.forceRenderingOff = on;
            if (mine == null) mine = GetComponent<MeshRenderer>();
            mine.enabled = on && Sources[0].enabled && Sources[0].gameObject.activeInHierarchy;
        }
    }
}
