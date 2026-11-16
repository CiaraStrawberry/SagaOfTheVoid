using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

// Bakes each recovered ship prefab to top-down sprites, one per team colour.
// Run: Unity -batchmode -quit -projectPath tools/bake -executeMethod ShipBaker.Run
public static class ShipBaker
{
    static readonly string[] Ships = {
        "Scout", "Fighter", "Bomber", "AttackCraft", "MissileFrigate", "FlackFrigate", "CannonFrigate",
        "LazerFrigate", "SupportFrigate", "RepairFrigate", "Destroyer", "Capital", "Super-Capital" };
    static readonly string[] Colours = { "Black", "Blue", "Green", "Grey", "Red", "White", "Yellow" };
    static readonly Regex ColourToken = new Regex("(Black|Blue|Green|Grey|Red|White|Yellow)");
    const int Size = 512;

    public static void Run()
    {
        var outDir = Path.Combine(Path.GetDirectoryName(Application.dataPath), "out");
        Directory.CreateDirectory(outDir);
        var texByName = new Dictionary<string, Texture2D>();
        foreach (var g in AssetDatabase.FindAssets("t:Texture2D"))
        {
            var t = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(g));
            if (t != null) texByName[t.name] = t;
        }
        Log("textures indexed: " + texByName.Count + ", unity " + Application.unityVersion);

        var standard = Shader.Find("Standard");
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.42f, 0.44f, 0.5f);
        RenderSettings.skybox = null;
        var sun = new GameObject("sun").AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.25f;
        sun.transform.rotation = Quaternion.Euler(60f, -35f, 0f);
        var cam = new GameObject("cam").AddComponent<Camera>();
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);
        cam.allowHDR = false;

        var manifest = new StringBuilder("{\n");
        bool firstShip = true;
        foreach (var name in Ships)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/" + name + ".prefab");
            if (prefab == null) { Log("MISSING prefab " + name); continue; }
            var go = Object.Instantiate(prefab);
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;

            // Only solid ship geometry: drop particles, lines, UI, sprites, text and shield bubbles.
            foreach (var c in go.GetComponentsInChildren<ParticleSystem>(true)) c.gameObject.SetActive(false);
            foreach (var c in go.GetComponentsInChildren<Canvas>(true)) c.gameObject.SetActive(false);
            foreach (var r in go.GetComponentsInChildren<Renderer>(true))
            {
                bool keep = (r is MeshRenderer || r is SkinnedMeshRenderer) && r.gameObject.activeInHierarchy
                    && !r.name.ToLower().Contains("shield") && !r.name.ToLower().Contains("outline")
                    && r.GetComponent<TextMesh>() == null;
                r.enabled = keep;
            }
            var live = go.GetComponentsInChildren<Renderer>().Where(r => r.enabled).ToList();
            if (live.Count == 0) { Log("NO RENDERERS " + name); Object.DestroyImmediate(go); continue; }

            // Rebuild materials on Standard, keeping the original textures.
            var originals = new Dictionary<Renderer, Material[]>();
            var notes = new HashSet<string>();
            foreach (var r in live) originals[r] = r.sharedMaterials;

            var b = live[0].bounds;
            foreach (var r in live) b.Encapsulate(r.bounds);
            float half = Mathf.Max(b.extents.x, b.extents.z) * 1.04f;
            cam.orthographicSize = half;
            cam.transform.position = new Vector3(b.center.x, b.max.y + 50f, b.center.z);
            cam.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = b.size.y + 100f;

            foreach (var colour in Colours)
            {
                foreach (var r in live)
                {
                    var src = originals[r];
                    var mats = new Material[src.Length];
                    for (int i = 0; i < src.Length; i++)
                        mats[i] = Rebuild(src[i], standard, colour, texByName, notes);
                    r.sharedMaterials = mats;
                }
                var png = Shoot(cam);
                File.WriteAllBytes(Path.Combine(outDir, name + "_" + colour + ".png"), png);
            }
            Log(string.Format("baked {0}: renderers={1} size=({2:F1} x {3:F1}) {4}", name, live.Count, b.size.x, b.size.z, string.Join(" | ", notes)));
            manifest.Append(firstShip ? "" : ",\n").AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "  \"{0}\": {{\"world_size\": {1:F3}, \"pixels\": {2}, \"renderers\": {3}}}", name, half * 2f, Size, live.Count);
            firstShip = false;
            Object.DestroyImmediate(go);
        }
        File.WriteAllText(Path.Combine(outDir, "manifest.json"), manifest.Append("\n}\n").ToString());
        Log("done");
    }

    static Material Rebuild(Material src, Shader standard, string colour, Dictionary<string, Texture2D> texByName, HashSet<string> notes)
    {
        var m = new Material(standard);
        if (src == null) return m;
        Texture main = Tex(src, "_MainTex", "_Albedo", "_BaseMap", "_MainTexture");
        if (main != null && ColourToken.IsMatch(main.name))
        {
            var swapped = ColourToken.Replace(main.name, colour, 1);
            if (texByName.TryGetValue(swapped, out var t)) main = t;
            else notes.Add("no " + swapped);
        }
        else if (main == null) notes.Add(src.name + ": no albedo (shader " + src.shader.name + ")");
        m.mainTexture = main;
        if (src.HasProperty("_Color")) m.color = src.color;
        var normal = Tex(src, "_BumpMap", "_Normal", "_NormalMap");
        if (normal != null) { m.EnableKeyword("_NORMALMAP"); m.SetTexture("_BumpMap", normal); }
        var glow = Tex(src, "_EmissionMap", "_Illum", "_Illumination", "_Emission");
        if (glow != null)
        {
            m.EnableKeyword("_EMISSION");
            m.SetTexture("_EmissionMap", glow);
            m.SetColor("_EmissionColor", Color.white);
        }
        m.SetFloat("_Glossiness", 0.25f);
        m.SetFloat("_Metallic", 0.1f);
        return m;
    }

    static Texture Tex(Material m, params string[] names)
    {
        foreach (var n in names)
            if (m.HasProperty(n) && m.GetTexture(n) != null) return m.GetTexture(n);
        return null;
    }

    static byte[] Shoot(Camera cam)
    {
        var rt = new RenderTexture(Size, Size, 24, RenderTextureFormat.ARGB32) { antiAliasing = 8 };
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;
        var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, Size, Size), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        cam.targetTexture = null;
        rt.Release();
        var bytes = tex.EncodeToPNG();
        Object.DestroyImmediate(tex);
        return bytes;
    }

    static void Log(string s) { Debug.Log("BAKE " + s); }
}
