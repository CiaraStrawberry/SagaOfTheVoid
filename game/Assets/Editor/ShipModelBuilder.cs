using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Saga.Game;
using Saga.Sim;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// One-off project tools, run without the editor window:
//   Unity -batchmode -quit -projectPath game -executeMethod ShipModelBuilder.SetupAndBuild
// 1. Makes a URP pipeline asset and switches the project to it.
// 2. Turns each recovered 2017 ship prefab into a clean prefab at Assets/Ships/<id>.prefab:
//    only the visible hull meshes, URP Lit materials, and a ShipModel component with all 7 team paints.
public static class ShipModelBuilder
{
    const string SrcPrefabs = "Assets/Imported/Prefabs/";
    const string OutDir = "Assets/Ships";
    const string MatDir = "Assets/Ships/Materials";
    static readonly Regex ColourToken = new Regex("(Black|Blue|Green|Grey|Red|White|Yellow)");

    public static void SetupAndBuild()
    {
        SetupUrp();
        BuildShips();
    }

    public static void SetupUrp()
    {
        Directory.CreateDirectory("Assets/Settings");
        var rendererPath = "Assets/Settings/GameRenderer.asset";
        var assetPath = "Assets/Settings/GamePipeline.asset";
        var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(rendererPath);
        if (renderer == null)
        {
            renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
            AssetDatabase.CreateAsset(renderer, rendererPath);
        }
        var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(assetPath);
        if (pipeline == null)
        {
            pipeline = UniversalRenderPipelineAsset.Create(renderer);
            AssetDatabase.CreateAsset(pipeline, assetPath);
        }
        pipeline.msaaSampleCount = 4;
        pipeline.supportsHDR = true;
        pipeline.renderScale = 1f;
        EditorUtility.SetDirty(pipeline);
        GraphicsSettings.defaultRenderPipeline = pipeline;
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            QualitySettings.SetQualityLevel(i, false);
            QualitySettings.renderPipeline = pipeline;
        }
        AssetDatabase.SaveAssets();
        Log("URP active: " + (GraphicsSettings.currentRenderPipeline != null) + " (" + GraphicsSettings.currentRenderPipeline?.name + ")");
    }

    public static void BuildShips()
    {
        AssetDatabase.DeleteAsset(MatDir); // rebuilt from scratch each run
        Directory.CreateDirectory(MatDir);
        AssetDatabase.Refresh();
        var defs = JsonUtility.FromJson<ShipDefList>(File.ReadAllText("Assets/Data/ships.json"));
        var texByName = new Dictionary<string, Texture2D>();
        foreach (var g in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Imported" }))
        {
            var t = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(g));
            if (t != null) texByName[t.name] = t;
        }
        var lit = Shader.Find("Universal Render Pipeline/Lit");
        if (lit == null) { Log("ERROR: URP Lit shader not found"); return; }
        var matCache = new Dictionary<string, Material>();

        foreach (var def in defs.ships)
        {
            var src = AssetDatabase.LoadAssetAtPath<GameObject>(SrcPrefabs + def.sprite + ".prefab");
            if (src == null) { Log("MISSING " + def.sprite); continue; }
            var tmp = (GameObject)Object.Instantiate(src);
            tmp.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            var root = new GameObject(def.id);
            var model = root.AddComponent<ShipModel>();
            var kept = new List<MeshRenderer>();
            var sourceMats = new List<Material>();
            var bounds = new Bounds();
            bool first = true;

            foreach (var r in tmp.GetComponentsInChildren<Renderer>(false))
            {
                if (!(r is MeshRenderer || r is SkinnedMeshRenderer)) continue;
                var lname = r.name.ToLower();
                if (lname.Contains("shield") || lname.Contains("outline") || r.GetComponent<TextMesh>() != null) continue;
                Mesh mesh = r is SkinnedMeshRenderer smr ? smr.sharedMesh : r.GetComponent<MeshFilter>()?.sharedMesh;
                if (mesh == null) continue;
                for (int slot = 0; slot < r.sharedMaterials.Length && slot < mesh.subMeshCount; slot++)
                {
                    var m = r.sharedMaterials[slot];
                    if (Albedo(m) == null) continue;
                    if (m.name.StartsWith("QuadMat")) continue; // flat selection marker from the VR game
                    var part = new GameObject(r.name + (slot > 0 ? "_" + slot : ""));
                    part.transform.SetParent(root.transform, false);
                    part.transform.SetPositionAndRotation(r.transform.position, r.transform.rotation);
                    part.transform.localScale = r.transform.lossyScale;
                    part.AddComponent<MeshFilter>().sharedMesh = mesh.subMeshCount > 1 ? SubMesh(mesh, slot, def.id) : mesh;
                    var mr = part.AddComponent<MeshRenderer>();
                    mr.shadowCastingMode = ShadowCastingMode.Off;
                    kept.Add(mr);
                    sourceMats.Add(m);
                    var wb = r.bounds;
                    if (first) { bounds = wb; first = false; } else bounds.Encapsulate(wb);
                }
            }
            Object.DestroyImmediate(tmp);

            var notes = new HashSet<string>();
            model.renderers = kept.ToArray();
            model.length = Mathf.Max(bounds.size.x, bounds.size.z);
            model.paints = ShipModel.Colours.Select(c => new ShipModel.Paint
            {
                colour = c,
                materials = sourceMats.Select(m => Convert(m, c, lit, texByName, matCache, notes)).ToArray(),
            }).ToArray();
            model.ApplyPaint("Grey");

            PrefabUtility.SaveAsPrefabAsset(root, OutDir + "/" + def.id + ".prefab");
            Object.DestroyImmediate(root);
            Log(string.Format("{0}: {1} parts, length {2:F1}{3}", def.id, kept.Count, model.length, notes.Count > 0 ? "  (" + string.Join("; ", notes) + ")" : ""));
        }
        AssetDatabase.SaveAssets();
        Log("done, " + matCache.Count + " materials");
        BuildLibrary(defs);
    }

    // The runtime's one entry point to ships, data, the effects shader and the sky.
    public static void BuildLibrary(ShipDefList defs)
    {
        Directory.CreateDirectory("Assets/Resources");
        var skyPath = "Assets/Settings/Skybox.mat";
        var sky = AssetDatabase.LoadAssetAtPath<Material>(skyPath);
        if (sky == null)
        {
            sky = new Material(Shader.Find("Skybox/Panoramic"));
            AssetDatabase.CreateAsset(sky, skyPath);
        }
        sky.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Backgrounds/skybox5.png"));
        sky.SetFloat("_Exposure", 0.45f);
        sky.SetFloat("_Rotation", 40f);
        EditorUtility.SetDirty(sky);

        var libPath = "Assets/Resources/ShipLibrary.asset";
        var lib = AssetDatabase.LoadAssetAtPath<ShipLibrary>(libPath);
        if (lib == null)
        {
            lib = ScriptableObject.CreateInstance<ShipLibrary>();
            AssetDatabase.CreateAsset(lib, libPath);
        }
        lib.ships = defs.ships.Select(d => new ShipLibrary.Entry
        {
            id = d.id,
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(OutDir + "/" + d.id + ".prefab"),
            icon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Icons/" + d.sprite + "_Blue.png"),
        }).ToList();
        lib.shipsJson = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Data/ships.json");
        lib.weaponsJson = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Data/weapons.json");
        lib.fxShader = Shader.Find("Saga/Fx");
        lib.skybox = sky;
        EditorUtility.SetDirty(lib);
        AssetDatabase.SaveAssets();
        Log(string.Format("library: {0} ships, {1} with prefab, {2} with icon, data {3}, shader {4}",
            lib.ships.Count, lib.ships.Count(e => e.prefab != null), lib.ships.Count(e => e.icon != null),
            lib.shipsJson != null && lib.weaponsJson != null, lib.fxShader != null));
    }

    // Splits one submesh out so each part has exactly one material slot.
    static Mesh SubMesh(Mesh mesh, int index, string shipId)
    {
        var path = MatDir + "/" + shipId + "_" + mesh.name + "_" + index + ".asset";
        var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (existing != null) return existing;
        var m = Object.Instantiate(mesh);
        m.SetTriangles(mesh.GetTriangles(index), 0);
        m.subMeshCount = 1;
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    static Material Convert(Material src, string colour, Shader lit, Dictionary<string, Texture2D> texByName, Dictionary<string, Material> cache, HashSet<string> notes)
    {
        var albedo = Albedo(src);
        if (albedo != null && ColourToken.IsMatch(albedo.name))
        {
            var swapped = ColourToken.Replace(albedo.name, colour, 1);
            if (texByName.TryGetValue(swapped, out var t)) albedo = t;
            else notes.Add("no " + swapped);
        }
        var key = (albedo != null ? albedo.name : src.name) + "|" + src.name;
        if (cache.TryGetValue(key, out var hit)) return hit;

        var m = new Material(lit) { name = albedo != null ? albedo.name.Replace("AlbedoAO", "") : src.name };
        m.SetTexture("_BaseMap", albedo);
        m.SetColor("_BaseColor", Color.white);
        var normal = Tex(src, "_BumpMap", "_Normal", "_NormalMap");
        if (normal != null)
        {
            var ni = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(normal)) as TextureImporter;
            if (ni != null && ni.textureType != TextureImporterType.NormalMap) { ni.textureType = TextureImporterType.NormalMap; ni.SaveAndReimport(); }
            m.SetTexture("_BumpMap", normal);
            m.EnableKeyword("_NORMALMAP");
        }
        var glow = Tex(src, "_EmissionMap", "_Illum", "_Illumination", "_Emission");
        if (glow != null)
        {
            m.SetTexture("_EmissionMap", glow);
            m.SetColor("_EmissionColor", Color.white * 1.6f);
            m.EnableKeyword("_EMISSION");
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        }
        m.SetFloat("_Metallic", 0.35f);
        m.SetFloat("_Smoothness", 0.45f);
        var path = MatDir + "/" + Safe(m.name) + "_" + Safe(src.name) + ".mat";
        AssetDatabase.CreateAsset(m, AssetDatabase.GenerateUniqueAssetPath(path));
        cache[key] = m;
        return m;
    }

    static Texture2D Albedo(Material m) => m == null ? null : Tex(m, "_MainTex", "_Albedo", "_BaseMap", "_MainTexture") as Texture2D;

    static Texture Tex(Material m, params string[] names)
    {
        foreach (var n in names)
            if (m.HasProperty(n) && m.GetTexture(n) != null) return m.GetTexture(n);
        return null;
    }

    static string Safe(string s) => Regex.Replace(s, @"[^\w\-]", "_");

    static void Log(string s) => Debug.Log("BUILD " + s);
}
