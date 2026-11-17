using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

// Sprite-stacking prototype: cuts each ship into horizontal slices (one per pixel of height) for the demo page
// docs/sprite-stack.html. Writes, per ship and team colour, the lit top view, its height map and the slice cross-sections at 4x the
// final pixel size; tools/sprite_stack.py turns them into pixel-art slice sheets.
// Run: Unity -batchmode -quit -projectPath tools/bake -executeMethod StackBaker.Run
public static class StackBaker
{
    // Ship, and its length in final pixels (sets the pixel-art scale).
    static readonly (string name, int px)[] Ships = {
        ("Scout", 22), ("Fighter", 26), ("Bomber", 30), ("AttackCraft", 30), ("MissileFrigate", 58), ("FlackFrigate", 58),
        ("CannonFrigate", 58), ("LazerFrigate", 60), ("SupportFrigate", 58), ("RepairFrigate", 58), ("Destroyer", 92),
        ("Capital", 132), ("Super-Capital", 170), ("SpaceStation", 170), ("turrets/CapitalMainGunInGame", 40) };
    static readonly string[] Squadrons = { "Scout", "Fighter", "Bomber", "AttackCraft" };
    static readonly string[] Colours = { "Blue", "Red" };
    const int Super = 4;
    const int LayersPerPixel = 2;   // two slices per pixel of hull height: smoother sides when drawn closer together

    public static void Run()
    {
        var outDir = Path.Combine(Path.GetDirectoryName(Application.dataPath), "out_stack");
        Directory.CreateDirectory(outDir);
        var texByName = new Dictionary<string, Texture2D>();
        foreach (var g in AssetDatabase.FindAssets("t:Texture2D"))
        {
            var t = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(g));
            if (t != null) texByName[t.name] = t;
        }
        var standard = Shader.Find("Standard");
        var slicer = AssetDatabase.LoadAssetAtPath<Shader>("Assets/Editor/StackSlice.shader");
        var heights = AssetDatabase.LoadAssetAtPath<Shader>("Assets/Editor/StackHeight.shader");
        if (slicer == null || heights == null) { Log("no slice or height shader"); return; }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.42f, 0.44f, 0.5f);
        RenderSettings.skybox = null;
        var sun = new GameObject("sun").AddComponent<Light>();
        sun.type = LightType.Directional; sun.intensity = 1.25f;
        sun.transform.rotation = Quaternion.Euler(60f, -35f, 0f);
        var cam = new GameObject("cam").AddComponent<Camera>();
        cam.orthographic = true; cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0); cam.allowHDR = false; cam.allowMSAA = false;

        var manifest = new StringBuilder("{\n");
        bool first = true;
        foreach (var (prefabName, px) in Ships)
        {
            var name = prefabName.Replace('/', '_');
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/" + prefabName + ".prefab");
            if (prefab == null) { Log("MISSING " + name); continue; }
            var go = Object.Instantiate(prefab);
            go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            foreach (var c in go.GetComponentsInChildren<ParticleSystem>(true)) c.gameObject.SetActive(false);
            foreach (var c in go.GetComponentsInChildren<Canvas>(true)) c.gameObject.SetActive(false);
            foreach (var r in go.GetComponentsInChildren<Renderer>(true))
                r.enabled = (r is MeshRenderer || r is SkinnedMeshRenderer) && r.gameObject.activeInHierarchy
                    && !r.name.ToLower().Contains("shield") && !r.name.ToLower().Contains("outline") && r.GetComponent<TextMesh>() == null;
            var live = go.GetComponentsInChildren<Renderer>().Where(r => r.enabled).ToList();
            // Squadrons (fighters, bombers, scouts) hold several craft, each under a child called "Ship": keep one.
            var craft = go.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "Ship" && t.GetComponentsInChildren<Renderer>().Any(r => r.enabled));
            if (craft != null && Squadrons.Contains(name))
            {
                foreach (var r in live) if (!r.transform.IsChildOf(craft)) r.enabled = false;
                live = live.Where(r => r.enabled).ToList();
            }
            if (live.Count == 0) { Object.DestroyImmediate(go); continue; }
            var originals = live.ToDictionary(r => r, r => r.sharedMaterials);

            var b = live[0].bounds;
            foreach (var r in live) b.Encapsulate(r.bounds);
            // Square frame around the ship; one final pixel = unit world units.
            float length = Mathf.Max(b.size.x, b.size.z);
            float unit = length / px;
            int side = Mathf.CeilToInt(length / unit) + 4;
            int slices = Mathf.Max(6, Mathf.CeilToInt(b.size.y / unit * LayersPerPixel));
            cam.orthographicSize = side * unit * 0.5f;
            float camY = b.max.y + 50f;
            cam.transform.position = new Vector3(b.center.x, camY, b.center.z);
            cam.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
            int res = side * Super;

            foreach (var colour in Colours)
            {
                foreach (var r in live)
                    r.sharedMaterials = originals[r].Select(m => ShipBakerAccess.Rebuild(m, standard, colour, texByName)).ToArray();
                cam.nearClipPlane = 0.1f; cam.farClipPlane = b.size.y + 100f;
                Save(Shoot(cam, res, null), Path.Combine(outDir, name + "_" + colour + "_top.png"));
                if (colour == Colours[0])
                {
                    Shader.SetGlobalFloat("_StackMinY", b.min.y); Shader.SetGlobalFloat("_StackSizeY", Mathf.Max(0.001f, b.size.y));
                    Save(Shoot(cam, res, heights), Path.Combine(outDir, name + "_height.png"));
                }
                for (int k = 0; k < slices; k++)
                {
                    // Slice k (0 = bottom) is cut through its middle height.
                    float h = b.min.y + (k + 0.5f) * b.size.y / slices;
                    cam.nearClipPlane = camY - h;
                    Save(Shoot(cam, res, slicer), Path.Combine(outDir, string.Format("{0}_{1}_s{2:D2}.png", name, colour, k)));
                }
            }
            Log(string.Format("{0}: {1}x{1} px, {2} slices", name, side, slices));
            manifest.Append(first ? "" : ",\n").AppendFormat(CultureInfo.InvariantCulture,
                "  \"{0}\": {{\"side\": {1}, \"slices\": {2}, \"super\": {3}, \"per_pixel\": {4}, \"unit\": {5:R}, \"cx\": {6:R}, \"cz\": {7:R}, \"miny\": {8:R}, \"sizey\": {9:R} }}",
                name, side, slices, Super, LayersPerPixel, unit, b.center.x, b.center.z, b.min.y, b.size.y);
            first = false;
            Object.DestroyImmediate(go);
        }
        File.WriteAllText(Path.Combine(outDir, "manifest.json"), manifest.Append("\n}\n").ToString());
        Log("done");
    }

    static Texture2D Shoot(Camera cam, int res, Shader replace)
    {
        var rt = new RenderTexture(res, res, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;
        if (replace != null) cam.RenderWithShader(replace, ""); else cam.Render();
        RenderTexture.active = rt;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, res, res), 0, 0); tex.Apply();
        RenderTexture.active = null; cam.targetTexture = null; rt.Release();
        return tex;
    }

    static void Save(Texture2D t, string path) { File.WriteAllBytes(path, t.EncodeToPNG()); Object.DestroyImmediate(t); }
    static void Log(string s) { Debug.Log("STACK " + s); }
}

// ShipBaker.Rebuild is private; same material rebuild, without the notes.
static class ShipBakerAccess
{
    static readonly System.Reflection.MethodInfo M = typeof(ShipBaker).GetMethod("Rebuild",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
    public static Material Rebuild(Material src, Shader standard, string colour, Dictionary<string, Texture2D> tex) =>
        (Material)M.Invoke(null, new object[] { src, standard, colour, tex, new HashSet<string>() });
}
