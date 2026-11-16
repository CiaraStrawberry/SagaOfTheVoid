using System.Collections.Generic;
using System.IO;
using System.Linq;
using Saga.Sim;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// Builds the three battle maps (2017 MapsPack1-3) as prefabs in Assets/Maps/<id>.prefab.
//   1. tools/mapbake: MapExtractor.Run exports meshes, textures and maps.json as maps.unitypackage
//   2. here:          Unity -batchmode -quit -projectPath game -executeMethod MapBuilder.ImportAndBuild
// Positions are the 2017 ones (maps sat in the same space as the ships). Heights are pushed below the
// battle plane so scenery never sits between the camera and the fleets.
public static class MapBuilder
{
    const string Src = "Assets/Imported/Maps";
    const string Out = "Assets/Maps";

    [System.Serializable] class Part { public string name, mesh, albedo, normal, emission; public float[] pos, rot, scale, colour; }
    [System.Serializable] class MapEntry { public string id; public List<Part> parts = new List<Part>(); }
    [System.Serializable] class Manifest { public List<MapEntry> maps = new List<MapEntry>(); }

    public static void ImportAndBuild()
    {
        var pkg = Path.GetFullPath(Path.Combine(Application.dataPath, "../../tools/mapbake/maps.unitypackage"));
        if (File.Exists(pkg))
        {
            AssetDatabase.ImportPackage(pkg, false);
            AssetDatabase.Refresh();
            Log("imported " + pkg);
        }
        Build();
    }

    // Below the plane: 1,200 units down plus a third of the old height difference, so layers keep some depth.
    static float FlattenY(float y) => -1200f - Mathf.Abs(y) * 0.33f;

    // The 2017 maps surrounded a VR player in the middle; from a strategy camera they sat off-screen.
    // Per map: pull the layout in by `spread` (positions and sizes), then grow each piece by `piece`.
    // Afterwards the whole map is lowered so its highest point stays under the battle plane.
    static readonly Dictionary<string, (float spread, float piece, float north, float shade)> Fit = new Dictionary<string, (float, float, float, float)>
    {
        { "asteroid_field", (0.45f, 2.4f, 0f, 0.5f) },     // rocks closer in and bigger
        { "space_station", (0.6f, 1f, 8000f, 0.3f) },     // half size, under the battle, darker (white hull)
        { "ship_graveyard", (0.45f, 1.0f, 0f, 0.6f) },     // derelict and debris field at the screen's sides
    };
    const float TopOfScenery = -600f;
    // shade: scenery brightness, below 1 so ships stand out

    [MenuItem("Saga/Rebuild battle maps")]
    public static void Build()
    {
        var manifestAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(Src + "/maps.json");
        if (manifestAsset == null) { Log("ERROR no " + Src + "/maps.json"); return; }
        var manifest = JsonUtility.FromJson<Manifest>(manifestAsset.text);
        AssetDatabase.DeleteAsset(Out);
        Directory.CreateDirectory(Out + "/Materials");
        AssetDatabase.Refresh();
        var lit = Shader.Find("Universal Render Pipeline/Lit");
        var mats = new Dictionary<string, Material>();

        foreach (var map in manifest.maps)
        {
            var root = new GameObject(map.id);
            int made = 0;
            foreach (var p in map.parts)
            {
                var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(p.mesh);
                if (mesh == null) { Log("missing mesh " + p.mesh); continue; }
                var fit = Fit.TryGetValue(map.id, out var f) ? f : (1f, 1f, 0f, 0.5f);
                var go = new GameObject(p.name);
                go.transform.SetParent(root.transform, false);
                go.transform.localPosition = new Vector3(p.pos[0], FlattenY(p.pos[1]), p.pos[2]) * fit.Item1 + Vector3.forward * fit.Item3;
                go.transform.localRotation = new Quaternion(p.rot[0], p.rot[1], p.rot[2], p.rot[3]);
                go.transform.localScale = new Vector3(p.scale[0], p.scale[1], p.scale[2]) * (fit.Item1 * fit.Item2);
                go.AddComponent<MeshFilter>().sharedMesh = mesh;
                var mr = go.AddComponent<MeshRenderer>();
                mr.shadowCastingMode = ShadowCastingMode.Off;
                mr.sharedMaterial = Material(p, lit, mats, fit.Item4);
                made++;
            }
            var b = new Bounds(); bool first = true;
            foreach (var r in root.GetComponentsInChildren<Renderer>()) { if (first) { b = r.bounds; first = false; } else b.Encapsulate(r.bounds); }
            float drop = Mathf.Min(0f, TopOfScenery - b.max.y);
            foreach (Transform c in root.transform) c.localPosition += Vector3.up * drop;
            b.center += Vector3.up * drop;
            PrefabUtility.SaveAsPrefabAsset(root, Out + "/" + map.id + ".prefab");
            Object.DestroyImmediate(root);
            Log(string.Format("{0}: {1} parts, bounds centre {2} size {3}", map.id, made, b.center, b.size));
        }
        AssetDatabase.SaveAssets();
        ShipModelBuilder.BuildLibrary(JsonUtility.FromJson<ShipDefList>(File.ReadAllText("Assets/Data/ships.json")));
    }

    // Diagnostic: each map alone, from straight above, framed on its bounds -> tools/preview/map_<id>.png
    public static void Snapshot()
    {
        var cam = new GameObject("cam").AddComponent<Camera>();
        cam.orthographic = true; cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(0.05f, 0.06f, 0.1f);
        var sun = new GameObject("sun").AddComponent<Light>(); sun.type = LightType.Directional; sun.intensity = 2f;
        sun.transform.rotation = Quaternion.Euler(60, -30, 0);
        RenderSettings.ambientMode = AmbientMode.Flat; RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.55f);
        var rt = new RenderTexture(1024, 1024, 24);
        var tex = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
        foreach (var id in new[] { "asteroid_field", "space_station", "ship_graveyard" })
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Out + "/" + id + ".prefab");
            var go = Object.Instantiate(prefab);
            var rs = go.GetComponentsInChildren<Renderer>();
            var b = rs[0].bounds; foreach (var r in rs) b.Encapsulate(r.bounds);
            var marker = GameObject.CreatePrimitive(PrimitiveType.Cube); // the battle area: 9,000 x 6,000 at the origin
            marker.transform.localScale = new Vector3(9000, 1, 6000);
            b.Encapsulate(marker.GetComponent<Renderer>().bounds);
            cam.orthographicSize = Mathf.Max(b.extents.x, b.extents.z) * 1.05f;
            cam.transform.SetPositionAndRotation(new Vector3(b.center.x, b.max.y + 1000, b.center.z), Quaternion.Euler(90, 0, 0));
            cam.farClipPlane = b.size.y + 5000;
            cam.targetTexture = rt; cam.Render();
            RenderTexture.active = rt; tex.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0); tex.Apply(); RenderTexture.active = null;
            File.WriteAllBytes("../tools/preview/map_" + id + ".png", tex.EncodeToPNG());
            int withTex = rs.Count(r => r.sharedMaterial != null && r.sharedMaterial.GetTexture("_BaseMap") != null);
            Log(string.Format("snapshot {0}: {1} renderers ({2} textured), bounds {3}, first mesh '{4}' size {5}",
                id, rs.Length, withTex, b.size, rs[0].GetComponent<MeshFilter>().sharedMesh?.name, rs[0].GetComponent<MeshFilter>().sharedMesh?.bounds.size));
            Object.DestroyImmediate(go); Object.DestroyImmediate(marker);
        }
    }

    static Material Material(Part p, Shader lit, Dictionary<string, Material> cache, float shade)
    {
        var key = p.albedo + "|" + p.normal + "|" + p.emission + "|" + shade;
        if (cache.TryGetValue(key, out var m)) return m;
        m = new Material(lit) { name = p.albedo != null ? Path.GetFileNameWithoutExtension(p.albedo) : "untextured" };
        var albedo = p.albedo != null ? AssetDatabase.LoadAssetAtPath<Texture2D>(p.albedo) : null;
        m.SetTexture("_BaseMap", albedo);
        m.SetColor("_BaseColor", new Color(p.colour[0] * shade, p.colour[1] * shade, p.colour[2] * shade, 1f));
        if (p.normal != null)
        {
            var ni = AssetImporter.GetAtPath(p.normal) as TextureImporter;
            if (ni != null && ni.textureType != TextureImporterType.NormalMap) { ni.textureType = TextureImporterType.NormalMap; ni.SaveAndReimport(); }
            m.SetTexture("_BumpMap", AssetDatabase.LoadAssetAtPath<Texture2D>(p.normal));
            m.EnableKeyword("_NORMALMAP");
        }
        if (p.emission != null)
        {
            m.SetTexture("_EmissionMap", AssetDatabase.LoadAssetAtPath<Texture2D>(p.emission));
            m.SetColor("_EmissionColor", Color.white);
            m.EnableKeyword("_EMISSION");
        }
        m.SetFloat("_Metallic", 0.1f);
        m.SetFloat("_Smoothness", 0.25f);
        AssetDatabase.CreateAsset(m, AssetDatabase.GenerateUniqueAssetPath(Out + "/Materials/" + m.name + ".mat"));
        cache[key] = m;
        return m;
    }

    static void Log(string s) => Debug.Log("MAPBUILD " + s);
}
