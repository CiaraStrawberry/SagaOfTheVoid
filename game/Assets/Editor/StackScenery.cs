using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

// Cuts the battle maps' scenery (asteroids, the station, wrecks) into sprite-stack slices, like the ships.
// The ships are cut by rendering in tools/bake; scenery only exists here (URP), so this works without rendering:
// for every pixel column it casts rays down and up through the piece's mesh, which gives the spans of height that
// are inside it; slices are filled there, coloured from the texture where the ray entered, lit by a fixed sun.
//
// Run: Unity -batchmode -quit -projectPath game -executeMethod StackScenery.Run
// Writes tools/scenery_out/<map>_<part>.png (colour; alpha 255 = hull, 200 = top surface) and scenery.json, which
// tools/sprite_stack.py turns into palette atlases in Art/Stacks and Data/scenery_stacks.json.
public static class StackScenery
{
    static readonly string[] Maps = { "asteroid_field", "space_station", "ship_graveyard" };
    const float MinUnit = 20f;          // world units per pixel at the finest (ship pixels are about 4-5)
    const long PartBudget = 1_100_000;  // most slice pixels per piece (one byte each in game)
    const long MapBudget = 6_000_000;   // and per map; past it every piece's pixels grow together
    const int LayersPerPixel = 2;
    static readonly Vector3 Sun = Quaternion.Euler(55f, -30f, 0f) * Vector3.forward; // direction the light travels

    public static void Run()
    {
        var outDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../../tools/scenery_out"));
        Directory.CreateDirectory(outDir);
        var json = new StringBuilder("{\"maps\":[");
        var readable = new Dictionary<Texture, Texture2D>();
        for (int mi = 0; mi < Maps.Length; mi++)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Maps/" + Maps[mi] + ".prefab");
            if (prefab == null) { Log("MISSING map " + Maps[mi]); continue; }
            var map = Object.Instantiate(prefab);
            map.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            map.transform.localScale = Vector3.one;
            var parts = map.GetComponentsInChildren<MeshRenderer>(true);

            // Pixel size per piece, as fine as its own budget allows (one huge piece doesn't coarsen the rest),
            // then all grown together if the map as a whole is over budget.
            float PartUnit(Vector3 bs) => Mathf.Max(MinUnit, (float)System.Math.Pow((double)bs.x * bs.z * bs.y * LayersPerPixel / PartBudget, 1.0 / 3.0));
            double estimate = parts.Sum(r => { var bs = r.bounds.size; float u = PartUnit(bs); return (double)bs.x / u * bs.z / u * bs.y / u * LayersPerPixel; });
            float grow = estimate > MapBudget ? (float)System.Math.Pow(estimate / MapBudget, 1.0 / 3.0) : 1f;
            json.Append(mi == 0 ? "" : ",").Append("{\"id\":\"").Append(Maps[mi]).Append("\",\"parts\":[");
            bool firstPart = true;
            long total = 0;
            for (int pi = 0; pi < parts.Length; pi++)
            {
                var r = parts[pi];
                var mf = r.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null) continue;
                float unit = PartUnit(r.bounds.size) * grow;
                var made = Cut(r, mf.sharedMesh, unit, readable, out var info);
                if (made == null) continue;
                var name = Maps[mi] + "_" + pi;
                File.WriteAllBytes(Path.Combine(outDir, name + ".png"), made.EncodeToPNG());
                Object.DestroyImmediate(made);
                total += (long)info.w * info.h * info.n;
                json.Append(firstPart ? "" : ",").AppendFormat(CultureInfo.InvariantCulture,
                    "{{\"sprite\":\"{0}\",\"part\":{1},\"slices\":{2},\"w\":{3},\"h\":{4},\"cols\":{5},\"rows\":{6},\"unit\":{7:R},\"left\":{8:R},\"top\":{9:R},\"miny\":{10:R},\"sizey\":{11:R} }}",
                    name, pi, info.n, info.w, info.h, info.cols, info.rows, unit, info.left, info.top, info.miny, info.sizey);
                firstPart = false;
            }
            json.Append("]}");
            Log(string.Format("{0}: {1} parts, pixels grown x{2:F2} for the map budget, {3:N0} slice pixels", Maps[mi], parts.Length, grow, total));
            Object.DestroyImmediate(map);
        }
        File.WriteAllText(Path.Combine(outDir, "scenery.json"), json.Append("]}").ToString());
        Log("done");
    }

    struct CutInfo { public int n, w, h, cols, rows; public float left, top, miny, sizey; }

    static Texture2D Cut(MeshRenderer r, Mesh mesh, float unit, Dictionary<Texture, Texture2D> readable, out CutInfo info)
    {
        info = default;
        var b = r.bounds;
        int w = Mathf.Max(1, Mathf.CeilToInt(b.size.x / unit)), h = Mathf.Max(1, Mathf.CeilToInt(b.size.z / unit));
        int n = Mathf.Max(2, Mathf.CeilToInt(b.size.y / unit * LayersPerPixel));
        float step = b.size.y / n;
        var col = r.gameObject.AddComponent<MeshCollider>();
        col.sharedMesh = mesh;
        Physics.SyncTransforms();

        var mat = r.sharedMaterial;
        Texture albedoTex = mat != null && mat.HasProperty("_BaseMap") ? mat.GetTexture("_BaseMap") : null;
        Color tint = mat != null && mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : Color.white;
        var albedo = Readable(albedoTex, readable);
        bool uvOk = mesh.isReadable && albedo != null;
        Color flat = albedo != null ? Average(albedo) : new Color(0.5f, 0.5f, 0.5f);

        var cells = new Color32[n][];
        for (int k = 0; k < n; k++) cells[k] = new Color32[w * h];
        int filled = 0;
        float top = b.max.y + 1f, bottom = b.min.y - 1f, reach = b.size.y + 2f;
        var entries = new List<(float y, Color c)>();
        var exits = new List<float>();
        for (int j = 0; j < h; j++)
            for (int i = 0; i < w; i++)
            {
                float x = b.min.x + (i + 0.5f) * unit, z = b.max.z - (j + 0.5f) * unit;
                entries.Clear(); exits.Clear();
                // Down: every surface facing up (the way into the hull from above).
                var o = new Vector3(x, top, z);
                for (int g = 0; g < 24 && col.Raycast(new Ray(o, Vector3.down), out var hit, top - bottom); g++)
                {
                    var c = (uvOk ? albedo.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y) : flat) * tint;
                    float light = 0.42f + 0.75f * Mathf.Max(0f, Vector3.Dot(hit.normal, -Sun));
                    entries.Add((hit.point.y, c * light));
                    o = hit.point + Vector3.down * 0.01f;
                }
                if (entries.Count == 0) continue;
                // Up: every surface facing down (the way out of the hull, going down).
                o = new Vector3(x, bottom, z);
                for (int g = 0; g < 24 && col.Raycast(new Ray(o, Vector3.up), out var hit, top - bottom); g++)
                {
                    exits.Add(hit.point.y);
                    o = hit.point + Vector3.up * 0.01f;
                }
                // Walk down the column: inside from an entry to the next exit below it.
                int e = 0;
                exits.Sort((p, q) => q.CompareTo(p));
                while (e < entries.Count)
                {
                    float y0 = entries[e].y;
                    var c = entries[e].c;
                    float y1 = y0 - step; // an open surface with nothing below: one slice thick
                    foreach (var xy in exits) if (xy < y0) { y1 = Mathf.Min(xy, y0 - step * 0.5f); break; }
                    int kTop = Mathf.Clamp(Mathf.FloorToInt((y0 - b.min.y) / step), 0, n - 1);
                    int kBot = Mathf.Clamp(Mathf.CeilToInt((y1 - b.min.y) / step - 0.5f), 0, kTop);
                    for (int k = kBot; k <= kTop; k++)
                    {
                        var px = (Color32)new Color(Mathf.Clamp01(c.r), Mathf.Clamp01(c.g), Mathf.Clamp01(c.b), k == kTop ? 200f / 255f : 1f);
                        if (cells[k][j * w + i].a == 0) { cells[k][j * w + i] = px; filled++; }
                    }
                    // Next entry below this span.
                    e++;
                    while (e < entries.Count && entries[e].y > y1) e++;
                }
            }
        Object.DestroyImmediate(col);
        if (filled == 0) return null;

        int cols = 1, rows = n;
        for (int cTry = 1, best = int.MaxValue; cTry <= n; cTry++)
        {
            int rTry = (n + cTry - 1) / cTry, size = Mathf.Max(cTry * w, rTry * h);
            if (size < best) { best = size; cols = cTry; rows = rTry; }
        }
        int W = cols * w, H = rows * h;
        var tex = new Texture2D(W, H, TextureFormat.RGBA32, false);
        var all = new Color32[W * H];
        for (int k = 0; k < n; k++)
        {
            int cr = k / cols, cc = k % cols;
            for (int j = 0; j < h; j++)
                for (int i = 0; i < w; i++)
                {
                    int py = cr * h + j;                 // PNG row, from the top
                    all[(H - 1 - py) * W + cc * w + i] = cells[k][j * w + i];
                }
        }
        tex.SetPixels32(all);
        tex.Apply();
        info = new CutInfo { n = n, w = w, h = h, cols = cols, rows = rows, left = b.min.x, top = b.max.z, miny = b.min.y, sizey = b.size.y };
        return tex;
    }

    static Texture2D Readable(Texture t, Dictionary<Texture, Texture2D> cache)
    {
        if (t == null) return null;
        if (cache.TryGetValue(t, out var have)) return have;
        // Small copy: a scenery pixel covers many texels, so this averages them (no speckle from fine detail).
        int w = Mathf.Min(128, t.width), h = Mathf.Min(128, t.height);
        var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        Graphics.Blit(t, rt);
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        var copy = new Texture2D(w, h, TextureFormat.RGBA32, false);
        copy.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        copy.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);
        cache[t] = copy;
        return copy;
    }

    static Color Average(Texture2D t)
    {
        var px = t.GetPixels32();
        long r = 0, g = 0, bl = 0;
        for (int i = 0; i < px.Length; i += 7) { r += px[i].r; g += px[i].g; bl += px[i].b; }
        int count = (px.Length + 6) / 7;
        return new Color(r / 255f / count, g / 255f / count, bl / 255f / count);
    }

    static void Log(string s) => Debug.Log("SCENERY " + s);
}
